using Azure;
using Azure.Data.Tables;
using ST10028058_CLDV6212_POE.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class TableStorageService
{
    private readonly TableClient _productTableClient;
    private readonly TableClient _customerTableClient;
    private readonly TableClient _orderTableClient;

    public TableStorageService(string connectionString)
    {
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new ArgumentNullException(nameof(connectionString), "Azure Storage connection string cannot be null or empty.");
        }

        _productTableClient = new TableClient(connectionString, "Products");
        _customerTableClient = new TableClient(connectionString, "Customers");
        _orderTableClient = new TableClient(connectionString, "Orders");
    }

    // Products
    public async Task<List<Product>> GetAllProductsAsync()
    {
        var products = new List<Product>();
        await foreach (var product in _productTableClient.QueryAsync<Product>())
        {
            products.Add(product);
        }
        return products;
    }

    public async Task AddProductAsync(Product product)
    {
        await _productTableClient.AddEntityAsync(product);
    }

    public async Task DeleteProductAsync(string partitionKey, string rowKey)
    {
        await _productTableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    public async Task<Product?> GetProductAsync(string partitionKey, string rowKey)
    {
        if (_productTableClient == null)
        {
            throw new InvalidOperationException("Table client is not initialized.");
        }

        try
        {
            var response = await _productTableClient.GetEntityAsync<Product>(partitionKey, rowKey);
            return response?.Value;
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            // The entity was not found
            return null;
        }
        catch (Exception ex)
        {
            // Log exception (you can log it here)
            throw new Exception("An error occurred while retrieving the product.", ex);
        }
    }

    public async Task UpdateProductAsync(Product product)
    {
        if (product == null)
        {
            throw new ArgumentNullException(nameof(product), "Product cannot be null.");
        }

        await _productTableClient.UpdateEntityAsync(product, product.ETag, TableUpdateMode.Replace);
    }

    // Customers
    public async Task<List<Customer>> GetAllCustomersAsync()
    {
        var customers = new List<Customer>();
        await foreach (var customer in _customerTableClient.QueryAsync<Customer>())
        {
            customers.Add(customer);
        }
        return customers;
    }

    public async Task AddCustomerAsync(Customer customer)
    {
        await _customerTableClient.AddEntityAsync(customer);
    }

    public async Task DeleteCustomerAsync(string partitionKey, string rowKey)
    {
        await _customerTableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    public async Task<Customer?> GetCustomerAsync(string partitionKey, string rowKey)
    {
        var response = await _customerTableClient.GetEntityAsync<Customer>(partitionKey, rowKey);
        return response?.Value;
    }

    // Orders
    public async Task<List<Order>> GetAllOrdersAsync()
    {
        var orders = new List<Order>();
        await foreach (var order in _orderTableClient.QueryAsync<Order>())
        {
            orders.Add(order);
        }
        return orders;
    }

    public async Task AddOrderAsync(Order order)
    {
        await _orderTableClient.AddEntityAsync(order);
    }

    public async Task DeleteOrderAsync(string partitionKey, string rowKey)
    {
        await _orderTableClient.DeleteEntityAsync(partitionKey, rowKey);
    }

    public async Task<Order?> GetOrderAsync(string partitionKey, string rowKey)
    {
        var response = await _orderTableClient.GetEntityAsync<Order>(partitionKey, rowKey);
        return response?.Value;
    }
}
