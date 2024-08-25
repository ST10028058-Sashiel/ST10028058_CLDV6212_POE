using Azure;
using Azure.Data.Tables;
using System.ComponentModel.DataAnnotations;

namespace ST10028058_CLDV6212_POE.Models
{
    public class Customer : ITableEntity
    {
        [Key]
        public int Customer_Id { get; set; }  
        public string? Customer_Name { get; set; }  
        public string? email { get; set; }
        public string? password { get; set; }

        // ITableEntity implementation
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }
}
