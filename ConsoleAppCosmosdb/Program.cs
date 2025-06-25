using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.Cosmos;
using ConsoleAppCosmosdb.Models;

class Program
{
    static async Task Main(string[] args)
    {
        string endpointUri = "https://ridesharewv64cosmos.documents.azure.com:443/";
        string primaryKey = "QOEaAqG9XwpWOQC1dkaihYORnAP89jmZVtX8eo5Dp929ofVA2trpqy4NUTYm85tdpD7RhU8f3SS5ACDbkPKtfw==";        
        string containerId = "products";

        CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
        CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey, options);

        Container products = cosmosClient.GetContainer("cosmicworkd", "products");

        List<Product> productsToInsert = new Faker<Product>()
            .StrictMode(true)
            .RuleFor(p => p.id, f => Guid.NewGuid().ToString())
            .RuleFor(p => p.categoryid, f => f.PickRandom(new[] { "1", "2", "3", "4", "5" }))
            .RuleFor(p => p.name, f => f.Commerce.ProductName())
            .RuleFor(p => p.price, f => Math.Round(f.Random.Double(10, 1000), 2))
            .RuleFor(p => p.tags, f => f.Commerce.Categories(3).ToArray())
            .Generate(2000);

        List<Task> concurrentTasks = new List<Task>();
        foreach (var product in productsToInsert)
        {
            concurrentTasks.Add(products.CreateItemAsync(product, new PartitionKey(product.categoryid)));
        }

        await Task.WhenAll(concurrentTasks);
        Console.WriteLine("Inserted 2000 products.");
    }
}