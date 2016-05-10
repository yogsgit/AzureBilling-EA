using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBillingAPI.Data
{
    public class UserSubscription :TableEntity
    {
        public UserSubscription()
        {
          
        }
        public UserSubscription(string id,string orgId)
        {
            this.RowKey = id;
            this.PartitionKey = orgId;
        }
        [IgnoreProperty]
        public string Id { get { return this.RowKey; } set { this.RowKey = value; } }
        public string DisplayName { get; set; }

        [IgnoreProperty]
        public string OrganizationId { get; set; }
        public bool IsConnected { get; set; }
        public DateTime ConnectedOn { get; set; }
        public string ConnectedBy { get; set; }
        public bool AzureAccessNeedsToBeRepaired { get; set; }
    }
}
