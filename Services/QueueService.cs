using Azure.Storage.Queues;
using System.Threading.Tasks;

public class QueueService
{
    private readonly QueueClient _queueClient;

    
    public QueueService(string connectionString, string queueName)
    {
        _queueClient = new QueueClient(connectionString, queueName);
    }

    public async Task SendMessageAsync(string message)
    {
        await _queueClient.SendMessageAsync(message);
    }
}


//# Assistance provided by ChatGPT
//# Code and support generated with the help of OpenAI's ChatGPT.
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