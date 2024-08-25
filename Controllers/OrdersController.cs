using ST10028058_CLDV6212_POE.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ST10028058_CLDV6212_POE.Services;

public class OrdersController : Controller
{
    private readonly TableStorageService _tableStorageService;
    private readonly QueueService _queueService;

    public OrdersController(TableStorageService tableStorageService, QueueService queueService)
    {
        _tableStorageService = tableStorageService;
        _queueService = queueService;
    }


    public async Task<IActionResult> Index()
    {
        var orders = await _tableStorageService.GetAllOrdersAsync();
    

        return View(orders); // Pass the list of OrderViewModel objects
    }



    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Order order)
    {
        if (ModelState.IsValid)
        {
            // Get the current max Order_Id and increment it
            var allOrders = await _tableStorageService.GetAllOrdersAsync();
            int maxOrderId = allOrders.Any() ? allOrders.Max(o => o.Order_Id) : 0;
            order.Order_Id = maxOrderId + 1;

            // Ensure Order_Date is in UTC
            order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);

            order.PartitionKey = "OrdersPartition";
            order.RowKey = Guid.NewGuid().ToString();

            await _tableStorageService.AddOrderAsync(order);
            return RedirectToAction("Index");
        }

        return View(order);
    }



    public async Task<IActionResult> Delete(string partitionKey, string rowKey)
    {
        await _tableStorageService.DeleteOrderAsync(partitionKey, rowKey);
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Details(string partitionKey, string rowKey)
    {
        var order = await _tableStorageService.GetOrderAsync(partitionKey, rowKey);
        if (order == null)
        {
            return NotFound();
        }
        return View(order);
    }


}
