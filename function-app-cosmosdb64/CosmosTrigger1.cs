using System;
using System.Collections.Generic;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace David64.CosmosFunction;

public class CosmosTrigger1
{
    [Function("CosmosTrigger1")]
    public static void Run([CosmosDBTrigger(
        databaseName: "cosmicworkd",
        containerName: "products",
        Connection = "ridesharewv64cosmos-DOCUMENTDB",
        LeaseContainerName = "productleases",
        CreateLeaseContainerIfNotExists = true)] IReadOnlyList<MyDocument> input,
        FunctionContext context)
    {
        var logger = context.GetLogger("CosmosTrigger1");

        if (input != null && input.Count > 0)
        {
            logger.LogInformation("Documents modified: " + input.Count);
            logger.LogInformation("First document Id: " + input[0].id);
        }
    }
}

public class MyDocument
{
    public required string id { get; set; }
    public required string categoryid { get; set; }
    public string? name { get; set; }
    public double? price { get; set; }
    public string[]? tags { get; set; }

    public MyDocument(){
        id = string.Empty;
        categoryid = string.Empty;
        name = null;
        price = null;
        tags = null;
    }
}