using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace TopicClient;

class Program
{
    static string EndPoint = "ps-servies.servicebus.windows.net";
    static (string Name, string Key) SasKeyManager = ("RootManageSharedAccessKey", "q2gwZdFCxe9pbXA4+KrizDyV3Ot2l8Kiz+ASbIg5zfA=");
    static (string Name, string Key) SasKeyWriter = ("Writer", "iUcHmWRy7ja2OhXtHjOPn3T96bQzMxy2e+ASbFZhWro=");
    static string TopicName = "onderwerp";

    static async Task Main(string[] args)
    {
        //await ManageTopicAsync();
        //await ManageSubscriptionsAsync();

        await WriteToTopicAsync();
        Console.WriteLine("Done");
        Console.ReadLine();
    }
    private static async Task ManageTopicAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyManager.Name, SasKeyManager.Key);
        var client = new ServiceBusAdministrationClient(EndPoint, cred);
        var topicInfo = new CreateTopicOptions(TopicName);

        var response = await client.CreateTopicAsync(topicInfo);
        if (response != null)
        {
            Console.WriteLine("Topic created!");
        }
    }
    private static async Task ManageSubscriptionsAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyManager.Name, SasKeyManager.Key);
        var client = new ServiceBusAdministrationClient(EndPoint, cred);
        
        var salesInfo = new CreateSubscriptionOptions(TopicName, "Sales");
        var salesRule = new CreateRuleOptions("sales", new SqlRuleFilter("price > 100"));
        await client.CreateSubscriptionAsync(salesInfo, salesRule);

        var supportInfo = new CreateSubscriptionOptions(TopicName, "Support");
        var supportRule = new CreateRuleOptions("support", new TrueRuleFilter());
        await client.CreateSubscriptionAsync(supportInfo, supportRule);

        var ceoInfo = new CreateSubscriptionOptions(TopicName, "Management");
        var ceoRule = new CreateRuleOptions("management", new SqlRuleFilter("price > 1000"));
        await client.CreateSubscriptionAsync(ceoInfo, ceoRule);

        Console.WriteLine("Subscriptions created");

    }

    private static async Task WriteToTopicAsync()
    {
        var cred = new AzureNamedKeyCredential(SasKeyWriter.Name, SasKeyWriter.Key);
        var client = new ServiceBusClient(EndPoint, cred);
        var sender = client.CreateSender(TopicName);

        Random rnd = new Random(); 
        for(int i = 0; i < 1000; i++)
        {
            var price = rnd.Next(0, 1500);
            var msg = new ServiceBusMessage(BinaryData.FromString($"Sold (${price})"));
            msg.ApplicationProperties.Add("price", price);
            msg.ContentType = "string";
            await sender.SendMessageAsync(msg);
        }
        
    }
}
