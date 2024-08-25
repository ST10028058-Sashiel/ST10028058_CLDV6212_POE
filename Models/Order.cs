using Azure;
using Azure.Data.Tables;
using System;

namespace ST10028058_CLDV6212_POE.Models
{
    public class Order : ITableEntity
    {
        public int Order_Id { get; set; }
        public int Product_ID { get; set; }
        public int Quantity { get; set; }
        public DateTime Order_Date { get; set; }

        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
