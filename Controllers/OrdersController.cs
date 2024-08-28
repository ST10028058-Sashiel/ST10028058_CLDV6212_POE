using Microsoft.AspNetCore.Mvc;
using ST10028058_CLDV6212_POE.Models;
using System.Threading.Tasks;
using System.Linq;

namespace ST10028058_CLDV6212_Part1.Controllers
{
    public class OrderController : Controller
    {
        private readonly TableStorageService _tableStorageService;
        private readonly QueueService _queueService;

        public OrderController(TableStorageService tableStorageService, QueueService queueService)
        {
            _tableStorageService = tableStorageService;
            _queueService = queueService;
        }

        // Display all orders
        public async Task<IActionResult> Index()
        {
            var orders = await _tableStorageService.GetAllOrdersAsync();
            return View(orders);
        }

        // Display the create order form
        public async Task<IActionResult> Create()
        {
            var customers = await _tableStorageService.GetAllCustomersAsync();
            var products = await _tableStorageService.GetAllProductsAsync();

            ViewData["Customers"] = customers;
            ViewData["Products"] = products;

            return View();
        }

        // Handle the form submission for creating a new order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Order order)
        {
            if (ModelState.IsValid)
            {
                var customers = await _tableStorageService.GetAllCustomersAsync();
                var products = await _tableStorageService.GetAllProductsAsync();

                // Find the selected customer and product
                var selectedCustomer = customers.FirstOrDefault(c => c.Customer_Name == order.CustomerName);
                var selectedProduct = products.FirstOrDefault(p => p.Product_Name == order.ProductName);

                if (selectedCustomer == null || selectedProduct == null)
                {
                    ModelState.AddModelError("", "Invalid customer or product selected.");
                    ViewData["Customers"] = customers;
                    ViewData["Products"] = products;
                    return View(order);
                }

                order.Customer_ID = selectedCustomer.Customer_Id;
                order.Product_ID = selectedProduct.Product_Id;
                order.PartitionKey = order.Customer_ID.ToString();
                order.RowKey = Guid.NewGuid().ToString();

                // Ensure Order_Date is in UTC
                order.Order_Date = DateTime.SpecifyKind(order.Order_Date, DateTimeKind.Utc);

                await _tableStorageService.AddOrderAsync(order);

                // Send a message to the queue
                string message = $"New order created with ID {order.Order_Id}, Customer {order.CustomerName}, Product {order.ProductName}.";
                await _queueService.SendMessageAsync(message);

                return RedirectToAction(nameof(Index));
            }

            var allCustomers = await _tableStorageService.GetAllCustomersAsync();
            var allProducts = await _tableStorageService.GetAllProductsAsync();
            ViewData["Customers"] = allCustomers;
            ViewData["Products"] = allProducts;

            return View(order);
        }

        // Display the details of a specific order
        public async Task<IActionResult> Details(string partitionKey, string rowKey)
        {
            var order = await _tableStorageService.GetOrderAsync(partitionKey, rowKey);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }


        // Display the delete confirmation page
        public async Task<IActionResult> Delete(string partitionKey, string rowKey)
        {
            var order = await _tableStorageService.GetOrderAsync(partitionKey, rowKey);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        // Handle the form submission for deleting an order
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string partitionKey, string rowKey)
        {
            var order = await _tableStorageService.GetOrderAsync(partitionKey, rowKey);
            if (order == null)
            {
                return NotFound();
            }

            await _tableStorageService.DeleteOrderAsync(partitionKey, rowKey);

            // Notify via queue
            string message = $"Order with ID {order.Order_Id}, Customer {order.CustomerName}, Product {order.ProductName} has been deleted.";
            await _queueService.SendMessageAsync(message);

            // Return to the confirmation view instead of redirecting
            return View("DeleteConfirmed", order);
        }

    }
}


//# Assistance provided by ChatGPT
//# Code and support generated with the help of OpenAI's ChatGPT.