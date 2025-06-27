using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bogus;
using Microsoft.Azure.Cosmos;
using static Microsoft.Azure.Cosmos.Container;
using Microsoft.Azure.Cosmos.Linq;
using System.ComponentModel;
using System.Reflection;
using ConsoleAppCosmosdb.Models;
using CosmosUtilities.KeyVault;


string endpointUri = await AzureUtility.GetRideshareCosmosEndPoint();
string primaryKey = await AzureUtility.GetRideshareCosmosPrimaryKey();


CosmosClientOptions options = new CosmosClientOptions { AllowBulkExecution = true };
CosmosClient cosmosClient = new CosmosClient(endpointUri, primaryKey, options);

Microsoft.Azure.Cosmos.Container products = cosmosClient.GetContainer("cosmicworkd", "products");

Microsoft.Azure.Cosmos.Container productlease = cosmosClient.GetContainer("cosmicworkd", "productlease");

ChangesHandler<Product> handleChanges = async (IReadOnlyCollection<Product> changes, CancellationToken cancellationToken) =>
{
    Console.WriteLine($"Started handling a batch of {changes.Count} changes...");
    foreach (Product product in changes)
    {
        await Console.Out.WriteLineAsync($"Detected operation for item with id {product.id}, name {product.name} and price {product.price}");
    }
};

var builder = products.GetChangeFeedProcessorBuilder<Product>(
    processorName:"productprocessor",
    onChangesDelegate: handleChanges
    );

ChangeFeedProcessor changeFeedProcessor = builder
    .WithInstanceName("consoleApp")
    .WithLeaseContainer(productlease)
    .Build();

    await changeFeedProcessor.StartAsync();

Console.WriteLine("Change Feed Processor started. Press any key to stop.");
Console.Read();

/*delete
var query = products.GetItemLinqQueryable<Product>().ToFeedIterator();

while (query.HasMoreResults)
{
    foreach (var item in await query.ReadNextAsync())
    {
        await products.DeleteItemAsync<Product>(item.id, new PartitionKey(item.categoryid));
    }
}
*/

/*Select
string sql = "SELECT * FROM products p WHERE p.price > 900";
QueryDefinition queryDefinition = new QueryDefinition(sql);
FeedIterator<Product> queryResultSetIterator = products.GetItemQueryIterator<Product>(queryDefinition); 
while (queryResultSetIterator.HasMoreResults)
{
    FeedResponse<Product> currentResultSet = await queryResultSetIterator.ReadNextAsync();
    foreach (Product product in currentResultSet)
    {
        Console.WriteLine($"Read {product.name} with price {product.price} in category {product.categoryid}  and id {product.id}");
    }
}


DatabaseResponse dbResponse = await cosmosClient.CreateDatabaseIfNotExistsAsync("cosmicworkd");
Microsoft.Azure.Cosmos.Database db = dbResponse.Database;

ContainerResponse containerResponse = await db.CreateContainerIfNotExistsAsync(new ContainerProperties
{
    Id = "products",
    PartitionKeyPath = "/categoryid"
});

Container products = containerResponse.Container;
*/
/*Bogus Faker
List<Product> productsToInsert = new Faker<Product>()
    .StrictMode(true)
    .RuleFor(p => p.id, f => Guid.NewGuid().ToString())
    .RuleFor(p => p.categoryid, f => f.PickRandom(new[] { "1", "2", "3", "4", "5" }))
    .RuleFor(p => p.name, f => f.Commerce.ProductName())
    .RuleFor(p => p.price, f => Math.Round(f.Random.Double(10, 1000), 2))
    .RuleFor(p => p.tags, f => f.Commerce.Categories(3).ToArray())
    .Generate(2500);

List<Task> concurrentTasks = new List<Task>();
foreach (var product in productsToInsert)
{
    concurrentTasks.Add(products.CreateItemAsync(product, new PartitionKey(product.categoryid)));
}

await Task.WhenAll(concurrentTasks);
Console.WriteLine("Inserted 2000 products.");
*/
