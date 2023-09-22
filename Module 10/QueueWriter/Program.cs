using Azure;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;

namespace QueueWriter;

class Program
{
    static string EndPoint = "ps-servies.servicebus.windows.net";
    static (string Name, string Key) NamedKeyManager = ("RootManageSharedAccessKey", "q2gwZdFCxe9pbXA4+KrizDyV3Ot2l8Kiz+ASbIg5zfA=");
    static (string Name, string Key)  NamedKeyWriter = ("Schrijven", "yFpwrLOnT7GyTTSsfNAkGxzdMF7E+NrWX+ASbBH/wQw=");
    static string QueueName = "myqueue";

    static async Task Main(string[] args)
    {
        //await ManageQueueAsync();
       await WriteToQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task WriteToQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(NamedKeyWriter.Name, NamedKeyWriter.Key);
        var options = new ServiceBusClientOptions
        {
            EnableCrossEntityTransactions = false,
            RetryOptions = new ServiceBusRetryOptions { Mode = ServiceBusRetryMode.Fixed },
            TransportType = ServiceBusTransportType.AmqpTcp
        };
        var client = new ServiceBusClient(EndPoint, cred, options);
        
        var sender = client.CreateSender(QueueName);

        int i = 0;
        ConsoleKey key;
        do
        {
            var msg = new ServiceBusMessage(BinaryData.FromString("Hello World " + i++));
            msg.ContentType = "string";
            msg.TimeToLive = TimeSpan.FromSeconds(300);
            msg.SessionId = "session id";
            msg.ReplyTo = "returnqueue";
           
            await sender.SendMessageAsync(msg);
            Console.WriteLine("Any key to send another message, Esc to quit");
            key = Console.ReadKey().Key;
        }
        while (key != ConsoleKey.Escape);

        var msgb = new ServiceBusMessage(BinaryData.FromString("Bye!!"));
        msgb.ContentType = "string";
        msgb.TimeToLive = TimeSpan.FromSeconds(30);
        await sender.SendMessageAsync(msgb);
    }

    private static async Task ManageQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(NamedKeyManager.Name, NamedKeyManager.Key);
        var client = new ServiceBusAdministrationClient(EndPoint, cred);
        var queueInfo = new CreateQueueOptions("returnqueue");
        queueInfo.AutoDeleteOnIdle = TimeSpan.FromMinutes(30);

        var response = await client.CreateQueueAsync(queueInfo);
        if (response != null)
        {
            Console.WriteLine("Queue created!");
        }
    }
}
