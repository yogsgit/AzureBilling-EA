using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Table.DataServices;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using AzureBillingAPI.Data;

namespace AzureUsageDataImport
{
    class Program
    {
        static void Main(string[] args)
        {
            string month = DateTime.UtcNow.Month < 10 ? "0" + DateTime.UtcNow.Month.ToString() : DateTime.UtcNow.Month.ToString();
            //string month = "04";
            string year = DateTime.UtcNow.Year.ToString();
            string requesturl = String.Format("https://billingapiaistest.azurewebsites.net/EaUsage/GetUsageData?yyyy={0}&mm={1}",
                year, month);

            DateTime startTime = DateTime.UtcNow;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requesturl);
            
            // Read Response
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Console.WriteLine(response.StatusDescription);
            StreamReader reader = new StreamReader(response.GetResponseStream());
            var jsonString =  reader.ReadToEnd();
            DateTime endTime = DateTime.UtcNow;

            JToken outer = JToken.Parse(jsonString);
            string runId = (string)outer["RunId"];

            EntityRepo<EAWebJobRunInfo> repo = new EntityRepo<EAWebJobRunInfo>();
            repo.Insert(new List<EAWebJobRunInfo>() {
                new EAWebJobRunInfo {
                    PartitionKey = year+"-"+month,
                    RowKey = runId,
                    RunId = runId,
                    StartTimeUTC = startTime,
                    EndTimeUTC = endTime
                }
            });


        }

    }
}
