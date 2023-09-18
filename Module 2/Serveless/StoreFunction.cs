using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace AzureFunctions;

public static class StoreFunction
{
    public class Person: TableEntity
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int Age { get; set; }
    }
    
    const string conStr = "DefaultEndpointsProtocol=https;AccountName=psstoor;AccountKey=jZcYA1z90g6Oqs+U+/NzwllMQb3cSUP7DlKWUOlB9IcMc1cZne60qUItQ3ZNTCXtv/P84o57z6VA+AStxE48xg==;EndpointSuffix=core.windows.net";
   
    
    [return: Table("people", Connection =conStr)]
    [FunctionName("StoreFunction")]
    public static Person Run(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "people/{first}/{last}/{age?}")] HttpRequest req,
        string first,
        string last,
        int? age,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        return new Person
        {
            PartitionKey = "viahttp",
            RowKey = Guid.NewGuid().ToString(),
            FirstName = first,
            LastName = last,
            Age = age ?? 42
        };        
    }

    [FunctionName("ServiceBusOutput")]
    [return: ServiceBus("myqueue", Connection = "ServiceBusConnection")]
    public static string ServiceBusOutput([HttpTrigger] dynamic input, ILogger log)
    {
        log.LogInformation($"C# function processed: {input.Text}");
        return input.Text;
    }
}
