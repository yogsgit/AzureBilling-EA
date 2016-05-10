using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBillingAPI.Data
{
    public class AzureUsageDetails : TableEntity
    {
        public AzureUsageDetails()
        { }

        public AzureUsageDetails(string meterId, string id)
        {
            this.RowKey = id;
            this.PartitionKey = meterId;
        }

        [IgnoreProperty]
        public string Id { get { return this.RowKey; } set { this.RowKey = value; } }

        public string Name { get; set; }

        public string Type { get; set; }

        public string SubscriptionId { get; set; }

        public string MeterId { get { return this.PartitionKey; } set { this.PartitionKey = value; } }

        public DateTime UsageStartTime { get; set; }

        public DateTime UsageEndTime { get; set; }

        public double Quantity { get; set; }

        public string Unit { get; set; }

        public string MeterName { get; set; }

        public string MeterCategory { get; set; }

        public string MeterSubCategory { get; set; }

        public string MeterRegion { get; set; }

        public DateTime DetailsDateTime { get; set; }

        public string PullId { get; set; }

    }

    public class AzureUsageDetailsAggregate : AzureUsageDetails
    {

    }
}
