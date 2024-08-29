using ST10028058_CLDV6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class CustomersController : Controller
{
    private readonly TableStorageService _tableStorageService;

    public CustomersController(TableStorageService tableStorageService)
    {
        _tableStorageService = tableStorageService;
    }

    public async Task<IActionResult> Index()
    {
        var customers = await _tableStorageService.GetAllCustomersAsync();
        return View(customers);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Customer customer)
    {
        // Get the current max Customer_Id and increment it
        var allCustomers = await _tableStorageService.GetAllCustomersAsync();
        int maxCustomerId = allCustomers.Any() ? allCustomers.Max(c => c.Customer_Id) : 0;
        customer.Customer_Id = maxCustomerId + 1;

        customer.PartitionKey = "CustomersPartition";
        customer.RowKey = Guid.NewGuid().ToString();

        await _tableStorageService.AddCustomerAsync(customer);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(string partitionKey, string rowKey)
    {
        await _tableStorageService.DeleteCustomerAsync(partitionKey, rowKey);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(string partitionKey, string rowKey)
    {
        var customer = await _tableStorageService.GetCustomerAsync(partitionKey, rowKey);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }
}


//# Assistance provided by ChatGPT
//# Code and support generated with the help of OpenAI's ChatGPT.
// code attribution
// W3schools
//https://www.w3schools.com/cs/index.php

// code attribution
//Microsoft
//https://learn.microsoft.com/en-us/aspnet/mvc/overview/getting-started/introduction/getting-started

// code attribution
//Microsoft
//https://learn.microsoft.com/en-us/azure/storage/blobs/storage-blob-dotnet-get-started?tabs=azure-ad

// code attribution
//Bootswatch
//https://bootswatch.com/