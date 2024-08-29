using Azure;
using Azure.Data.Tables;
using System;
using System.ComponentModel.DataAnnotations;

namespace ST10028058_CLDV6212_POE.Models
{
    public class Order : ITableEntity
    {
        [Key]
        public int Order_Id { get; set; }
        public string? PartitionKey { get; set; }
        public string? RowKey { get; set; }
        public ETag ETag { get; set; }
        public DateTimeOffset? Timestamp { get; set; }

        [Required(ErrorMessage = "Select a Customer")]
        public int Customer_ID { get; set; }

        [Required(ErrorMessage = "Select a Product")]
        public int Product_ID { get; set; }

        [Required(ErrorMessage = "Enter a valid Date")]
        public DateTime Order_Date { get; set; }

        [Required(ErrorMessage = "Enter the location")]
        public string? Order_Address { get; set; }

        // New properties for storing names
        public string? CustomerName { get; set; }
        public string? ProductName { get; set; }
    }
}
//# Assistance provided by ChatGPT
//# Code and support generated with the help of OpenAI's ChatGPT.
// code attribution
// W3schools
// https://www.w3schools.com/cs/index.php

// code attribution
//Microsoft
//https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/getting-started

// code attribution
//Microsoft
//https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-dotnet-get-started?tabs=azure-ad

// code attribution
//Bootswatch
//https://bootswatch.com/