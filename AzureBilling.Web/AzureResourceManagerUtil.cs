using AzureBillingAPI.Web.Models;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using AzureBillingAPI.Data;

namespace AzureBillingAPI.Web
{
    public static class AzureResourceManagerUtil
    {
        public static List<Organization> GetUserOrganizations()
        {
            List<Organization> organizations = new List<Organization>();

            string tenantId = ConfigurationManager.AppSettings["ida:TenantID"];
            try
            {
                AuthenticationResult result = GetFreshAuthToken(tenantId);
                
                // Get a list of Organizations of which the user is a member            
                string requestUrl = string.Format("{0}/tenants?api-version={1}", ConfigurationManager.AppSettings["ida:AzureResourceManagerUrl"],
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerAPIVersion"]);

                // Make the GET request
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var organizationsResult = (Json.Decode(responseContent)).value;

                    foreach (var organization in organizationsResult)
                        organizations.Add(new Organization()
                        {
                            Id = organization.tenantId,
                            objectIdOfISVDemoUsageServicePrincipal =
                                AzureADGraphAPIUtil.GetObjectIdOfServicePrincipalInOrganization(organization.tenantId, ConfigurationManager.AppSettings["ida:ClientID"])
                        });
                }
            }
            catch
            {
                
            }
            return organizations;
        }
        
        public static List<Subscription> GetUserSubscriptions(string organizationId)
        {
            List<Subscription> subscriptions = null;
            try
            {
                AuthenticationResult result = GetFreshAuthToken(organizationId);

                subscriptions = new List<Subscription>();

                // Get subscriptions to which the user has some kind of access
                string requestUrl = string.Format("{0}/subscriptions?api-version={1}",
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerUrl"],
                    ConfigurationManager.AppSettings["ida:AzureResourceManagerAPIVersion"]);

                // Make the GET request
                HttpClient client = new HttpClient();
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, requestUrl);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.AccessToken);
                HttpResponseMessage response = client.SendAsync(request).Result;

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = response.Content.ReadAsStringAsync().Result;
                    var subscriptionsResult = (Json.Decode(responseContent)).value;

                    foreach (var subscription in subscriptionsResult)
                        subscriptions.Add(new Subscription()
                        {
                            Id = subscription.subscriptionId,
                            DisplayName = subscription.displayName,
                            OrganizationId = organizationId
                        });
                }
            }
            catch(Exception exp)
            {
                //log exceptions
            }
            return subscriptions;
        }

        public static string GetBilling(string subscriptionId, string organizationId, string offerId, string currency, string language, string regionInfo)
        {
            string usageText = "";
            try
            {
                AuthenticationResult result = GetFreshAuthToken(organizationId);

                // Making a call to the Azure Usage API for a set time frame with the input AzureSubID
                string requesturl = String.Format("https://management.azure.com/subscriptions/{0}/providers/Microsoft.Commerce/RateCard?api-version={1}&$filter=OfferDurableId eq '{2}' and Currency eq '{3}' and Locale eq '{4}' and RegionInfo eq '{5}'",
                    subscriptionId,
                    "2015-06-01-preview",
                    offerId,
                    currency,
                    language,
                    regionInfo);

                // HTTP call
                usageText = GetJson(result, requesturl);
            }
            catch
            {
                // log error into the table storage logs
            }
            return usageText;
        }

        private static string GetJson(AuthenticationResult result, string requesturl)
        {
            string usageText;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturl);
            request.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + result.AccessToken);
            request.ContentType = "application/json";

            // Read Response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(response.StatusDescription);
            Stream receiveStream = response.GetResponseStream();

            // read stream as text
            StreamReader responseStream = new StreamReader(receiveStream, Encoding.UTF8);
            usageText = responseStream.ReadToEnd();
            return usageText;
        }

        private static AuthenticationResult GetFreshAuthToken(string organizationId)
        {
            // Aquire Access Token to call Azure Resource Manager
            string signedInUserUniqueName = ConfigurationManager.AppSettings["ida:Username"];
            ClientCredential credential = new ClientCredential(ConfigurationManager.AppSettings["ida:ClientID"],
                ConfigurationManager.AppSettings["ida:Password"]);
            AuthenticationContext authContext = new AuthenticationContext(
                string.Format(ConfigurationManager.AppSettings["ida:Authority"], organizationId), new ADALTokenCache(signedInUserUniqueName));
            AuthenticationResult result = authContext.AcquireToken(ConfigurationManager.AppSettings["ida:AzureResourceManagerIdentifier"], credential);
            return result;
        }

        public static string GetUsage(string subscriptionId, string organizationId,string startTime, string endTime)
        {
            string usageText = "";
            try
            {
                AuthenticationResult result = GetFreshAuthToken(organizationId);

                // Making a call to the Azure Usage API for a set time frame with the input AzureSubID
                string requesturl = String.Format("https://management.azure.com/subscriptions/{0}/providers/Microsoft.Commerce/UsageAggregates?api-version=2015-06-01-preview&reportedstartTime={1}&reportedEndTime={2}", subscriptionId,
                    startTime,
                    endTime);

                // HTTP call
                usageText = GetJson(result, requesturl);
                //dynamic data = Json.Decode(usageText);
                UsageDetailsTemp usage = Json.Decode<UsageDetailsTemp>(usageText);
                List<AzureUsageDetails> usageDetails = new List<AzureUsageDetails>();
                DateTime detailsDateTime = DateTime.Now;
                string pullId = Guid.NewGuid().ToString();
                if (usage != null && usage.value != null)
                {
                    foreach (Value val in usage.value)
                    {
                        AzureUsageDetails newDetail = new AzureUsageDetails(val.properties.meterId, Guid.NewGuid().ToString());
                        newDetail.Name = val.name;
                        newDetail.Type = val.type;
                        newDetail.SubscriptionId = val.properties.subscriptionId;
                        newDetail.UsageStartTime = Convert.ToDateTime(val.properties.usageStartTime);
                        newDetail.UsageEndTime = Convert.ToDateTime(val.properties.usageEndTime);
                        newDetail.Quantity = val.properties.quantity;
                        newDetail.Unit = val.properties.unit;
                        newDetail.MeterCategory = val.properties.meterCategory;
                        newDetail.MeterSubCategory = val.properties.meterSubCategory;
                        newDetail.MeterName = val.properties.meterName;
                        newDetail.MeterRegion = val.properties.infoFields.meteredRegion;
                        newDetail.DetailsDateTime = detailsDateTime;
                        newDetail.PullId = pullId;
                        usageDetails.Add(newDetail);
                    }
                }
                var aggregateUsage = from us in usageDetails
                                       group us by new
                                       {
                                           us.MeterId,
                                           us.MeterName,
                                           us.MeterCategory,
                                           us.MeterSubCategory,
                                           us.Unit
                                       }
                                       into fus
                                       select new AzureUsageDetailsAggregate()
                                       {
                                           Quantity = fus.Sum(x => x.Quantity),
                                           Name = fus.FirstOrDefault().Name,
                                           Type = fus.FirstOrDefault().Type,
                                           SubscriptionId = fus.FirstOrDefault().SubscriptionId,
                                           MeterId = fus.Key.MeterId,
                                           UsageStartTime = fus.FirstOrDefault().UsageStartTime,
                                           UsageEndTime = fus.FirstOrDefault().UsageEndTime,
                                           Unit = fus.Key.Unit,
                                           MeterName = fus.Key.MeterName,
                                           MeterCategory = fus.Key.MeterCategory,
                                           MeterSubCategory = fus.Key.MeterSubCategory,
                                           MeterRegion = fus.FirstOrDefault().MeterRegion,
                                           DetailsDateTime = fus.FirstOrDefault().DetailsDateTime,
                                           Id = fus.FirstOrDefault().Id,
                                           PullId = fus.FirstOrDefault().PullId
                                       };
                EntityRepo<AzureUsageDetails> usageEntityRepo = new EntityRepo<AzureUsageDetails>();
                usageEntityRepo.Insert(usageDetails);
                EntityRepo<AzureUsageDetailsAggregate> usageEntityRepoAgg = new EntityRepo<AzureUsageDetailsAggregate>();
                usageEntityRepoAgg.Insert(aggregateUsage.ToList());
            }
            catch(Exception e)
            {
                // log error into the table storage logs
            }

            return usageText;
        }
       
    }
}