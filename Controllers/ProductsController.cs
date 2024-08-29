using ST10028058_CLDV6212_POE.Models;
using ST10028058_CLDV6212_POE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ProductsController : Controller
{
    private readonly BlobService _blobService;
    private readonly TableStorageService _tableStorageService;
  

    public ProductsController(BlobService blobService, TableStorageService tableStorageService)
    {
        _blobService = blobService;
        _tableStorageService = tableStorageService;
       
    }

    public async Task<IActionResult> Index()
    {
        var products = await _tableStorageService.GetAllProductsAsync();
        return View(products);
    }

    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Product product, IFormFile file)
    {
        if (file != null)
        {
            using var stream = file.OpenReadStream();
            var imageUrl = await _blobService.UploadAsync(stream, file.FileName);
            product.ImageUrl = imageUrl;
        }

        var allProducts = await _tableStorageService.GetAllProductsAsync();
        int maxProductId = allProducts.Any() ? allProducts.Max(p => p.Product_Id) : 0;
        product.Product_Id = maxProductId + 1;

        product.PartitionKey = "ProductsPartition";
        product.RowKey = Guid.NewGuid().ToString();

        await _tableStorageService.AddProductAsync(product);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(string partitionKey, string rowKey)
    {
        var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);
    }

    
    public async Task<IActionResult> Delete(string partitionKey, string rowKey)
    {
        var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
        if (product != null && !string.IsNullOrEmpty(product.ImageUrl))
        {
            await _blobService.DeleteBlobAsync(product.ImageUrl);
        }

        await _tableStorageService.DeleteProductAsync(partitionKey, rowKey);
        return RedirectToAction("Index");
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