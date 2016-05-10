using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBillingAPI.Data
{
    public class EAWebJobRunInfo : TableEntity
    {
        public EAWebJobRunInfo() {

        }

        public string RunId { get; set; }

        public DateTime StartTimeUTC { get; set; }
        public DateTime EndTimeUTC { get; set; }
    }
}
