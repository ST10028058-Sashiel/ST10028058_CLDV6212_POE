using Azure;
using Azure.Data.Tables;
using System;
using System.ComponentModel.DataAnnotations;

namespace ST10028058_CLDV6212_POE.Models
{
    public class Product : ITableEntity
    {
        [Key]
        public int Product_Id { get; set; }  
        public string? Product_Name { get; set; }  
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Product_Price { get; set; } 
        public int Quantity { get; set; }

        // ITableEntity implementation
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
    }

}
