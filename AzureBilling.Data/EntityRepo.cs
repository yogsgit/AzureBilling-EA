using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureBillingAPI.Data
{
    //public class EntityRepo<T> where T: TableEntity
    public class EntityRepo<T> where T : TableEntity, new()
    {
        private CloudTableClient tableClient = null;
        private CloudTable table = null;

        public EntityRepo()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
            tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(typeof(T).Name);
            table.CreateIfNotExists();
        }

        public void Insert(List<T> enities)
        {
            foreach (var item in enities)
            {
                var entityItem = item as TableEntity;
                TableOperation insertOperation = TableOperation.InsertOrReplace(entityItem);
                table.Execute(insertOperation);
            }

        }

        public void Delete(T entity)
        {
            var entityItem = entity as TableEntity;
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, entityItem.PartitionKey);
            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, entityItem.RowKey);
            string aggregate = TableQuery.CombineFilters(partitionKeyFilter, "and", rowKeyFilter);
            var existingPartitionData = new TableQuery().Where(aggregate);
            var item = table.ExecuteQuery(existingPartitionData).FirstOrDefault();
            if(item != null)
            {
                TableOperation deleteOperation = TableOperation.Delete(item);
                table.Execute(deleteOperation);
            }

        }

        public T Get(string rowKey, string partitionKey)
        {
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);
            string aggregate = TableQuery.CombineFilters(partitionKeyFilter, "and", rowKeyFilter);
            var existingPartitionData = new TableQuery().Where(aggregate);
            var item = table.ExecuteQuery(existingPartitionData).FirstOrDefault();
            return item as T;
        }

        public IList<T> Get(string partitionKey,List<Tuple<string,string>> keyValuePair, string rowKey="")
        {
            string partitionKeyFilter = TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, partitionKey);
            List<string> criteria = new List<string>();
            foreach(var item in keyValuePair)
            {

                string criterian = TableQuery.CombineFilters(item.Item1, QueryComparisons.Equal, "'"+item.Item2+ "'");
                criteria.Add(criterian);
            }
            string aggregate = partitionKeyFilter;
            if (criteria.Count > 0)
            {
                foreach (var criterian in criteria)
                {
                    aggregate = TableQuery.CombineFilters(partitionKeyFilter, "and", criterian);
                }
            }
            if (!string.IsNullOrEmpty(rowKey))
            {
                string rowKeyFilter = TableQuery.GenerateFilterCondition("RowKey", QueryComparisons.Equal, rowKey);
                aggregate = TableQuery.CombineFilters(aggregate, "and", rowKeyFilter);
            }
           // aggregate = TableQuery.CombineFilters(aggregate, "and", partitionKeyFilter);
            var existingPartitionData = new TableQuery<T>().Where(aggregate);
            return table.ExecuteQuery(existingPartitionData).ToList();
            
        }

    }
}
