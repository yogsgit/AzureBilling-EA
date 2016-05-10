using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBillingAPI.Data
{
    public class EAUsageAccountSummaryEntity : TableEntity
    {
        public EAUsageAccountSummaryEntity() { }

        public string AccountName { get; set; }
        public string RunId { get; set; }
        public double Amount { get; set; }
    }
}
