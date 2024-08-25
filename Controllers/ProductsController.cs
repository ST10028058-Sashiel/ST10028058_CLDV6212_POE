using ST10028058_CLDV6212_POE.Models;
using ST10028058_CLDV6212_POE.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

public class ProductsController : Controller
{
    private readonly BlobService _blobService;
    private readonly TableStorageService _tableStorageService;
    private readonly QueueService _queueService;

    public ProductsController(BlobService blobService, TableStorageService tableStorageService, QueueService queueService)
    {
        _blobService = blobService;
        _tableStorageService = tableStorageService;
        _queueService = queueService;
    }

    public async Task<IActionResult> Index()
    {
        var products = await _tableStorageService.GetAllProductsAsync();
        return View(products);
    }

    [HttpGet]
    public IActionResult CreateOrder(int productId)
    {
        var order = new Order
        {
            Product_ID = productId,
            Quantity = 1 // Default quantity
        };

        return View(order);
    }

    [HttpPost]
    public async Task<IActionResult> SubmitOrder(Order order)
    {
        var product = await _tableStorageService.GetProductAsync("ProductsPartition", order.Product_ID.ToString());
        if (product == null || product.Quantity < order.Quantity)
        {
            ModelState.AddModelError("", "Product not found or insufficient quantity.");
            return View(order);
        }

        order.RowKey = Guid.NewGuid().ToString();  // Unique identifier
        order.Order_Date = DateTime.UtcNow;
        order.PartitionKey = "OrdersPartition";

        // Send the order to the queue
        var orderJson = Newtonsoft.Json.JsonConvert.SerializeObject(order);
        await _queueService.SendMessageAsync(orderJson);

        // Update the product quantity
        product.Quantity -= order.Quantity;
        await _tableStorageService.UpdateProductAsync(product);

        // Save the order to the table
        await _tableStorageService.AddOrderAsync(order);

        return RedirectToAction("Index", "Orders");
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

    public async Task<IActionResult> OrderProduct(string partitionKey, string rowKey)
    {
        var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
        if (product == null)
        {
            return NotFound();
        }

        var order = new Order
        {
            Product_ID = product.Product_Id,
            Order_Date = DateTime.UtcNow,
            PartitionKey = "OrdersPartition",
            RowKey = Guid.NewGuid().ToString(),
            Quantity = 1 // Default quantity
        };

        return View(order); // Ensure this view expects an Order model
    }

    [HttpPost]
    public async Task<IActionResult> OrderProduct(string partitionKey, string rowKey, int quantity)
    {
        var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
        if (product == null)
        {
            return NotFound();
        }

        if (product.Quantity < quantity)
        {
            ModelState.AddModelError("", "Insufficient product quantity.");
            return View(new Order
            {
                Product_ID = product.Product_Id,
                Quantity = quantity,
                Order_Date = DateTime.UtcNow,
                PartitionKey = "OrdersPartition",
                RowKey = Guid.NewGuid().ToString()
            });
        }

        var order = new Order
        {
            Product_ID = product.Product_Id,
            Quantity = quantity,
            Order_Date = DateTime.UtcNow,
            PartitionKey = "OrdersPartition",
            RowKey = Guid.NewGuid().ToString()
        };

        var allOrders = await _tableStorageService.GetAllOrdersAsync();
        int maxOrderId = allOrders.Any() ? allOrders.Max(o => o.Order_Id) : 0;
        order.Order_Id = maxOrderId + 1;

        await _tableStorageService.AddOrderAsync(order);

        // Update the product quantity
        product.Quantity -= quantity;
        await _tableStorageService.UpdateProductAsync(product);

        return RedirectToAction("Index", "Orders");
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

    public async Task<IActionResult> CreateOrderWithDetails(string partitionKey, string rowKey)
    {
        var product = await _tableStorageService.GetProductAsync(partitionKey, rowKey);
        if (product == null)
        {
            return NotFound();
        }

        var order = new Order
        {
            Product_ID = product.Product_Id, // Use the Product_Id from the retrieved product
            Order_Date = DateTime.UtcNow,
            PartitionKey = "OrdersPartition",
            RowKey = Guid.NewGuid().ToString(),
            Quantity = 1 // Default quantity
        };

        return View(order); // Ensure this view expects an Order model
    }
}
