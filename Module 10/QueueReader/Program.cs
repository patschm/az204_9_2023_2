using Azure;
using Azure.Messaging.ServiceBus;

namespace QueueReader;

class Program
{
    static string EndPoint = "ps-servies.servicebus.windows.net";
    static (string Name, string Key) NamedKeyReader = ("Lezen", "Nn/Lf6GJzvbhoWAZmydDfgnf7Y6ToB5ne+ASbPyXsOw=");
    static string QueueName = "myqueue";

    static async Task Main(string[] args)
    {
        //await ReadQueueAsync();
        await ReadSessionQueueAsync();
        //await ReadQueueProcessorAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task ReadQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(NamedKeyReader.Name, NamedKeyReader.Key);
        var client = new ServiceBusClient(EndPoint, cred);
        var options = new ServiceBusProcessorOptions
        {
            ReceiveMode = ServiceBusReceiveMode.PeekLock
        };
        var receiver = client.CreateReceiver(QueueName);
        int i = 0;
        do
        {
            var msg = await receiver.ReceiveMessageAsync();
             if (i++ % 5 == 0)
            {
                //await receiver.AbandonMessageAsync(msg);
                continue;
            }
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
            var data = msg.Body.ToString();
            Console.WriteLine(data);
           
           await receiver.CompleteMessageAsync(msg);
            //await receiver.AbandonMessageAsync(msg);
            //await receiver.RenewMessageLockAsync(msg);
            await Task.Delay(100);
        }
        while (true);
    }
    private static async Task ReadSessionQueueAsync()
    {
        var cred = new AzureNamedKeyCredential(NamedKeyReader.Name, NamedKeyReader.Key);
        var client = new ServiceBusClient(EndPoint, cred);
        var receiver = await client.AcceptSessionAsync(QueueName, "session id");
        int i = 0;
        do
        {
            var msg = await receiver.ReceiveMessageAsync();
            if (++i % 5 == 0)
            {
                //await receiver.AbandonMessageAsync(msg);
                throw new Exception("Ooops");
            }
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
            var data = msg.Body.ToString();
            Console.WriteLine(data);
            await receiver.CompleteMessageAsync(msg);
            //await receiver.AbandonMessageAsync(msg);
            //await receiver.RenewMessageLockAsync(msg);
            await Task.Delay(100);
        }
        while (true);
    }
    private static async Task ReadQueueProcessorAsync()
    {
        var cred = new AzureNamedKeyCredential(NamedKeyReader.Name, NamedKeyReader.Key);
        var client = new ServiceBusClient(EndPoint, cred);
        var receiver = client.CreateProcessor(QueueName);
        
        receiver.ProcessMessageAsync += async evtArg => {
            var msg = evtArg.Message;
            Console.WriteLine($"Lock Duration: {msg.LockedUntil} Lock Token: {msg.LockToken}");
            var data = msg.Body.ToString();
            Console.WriteLine(data);
            await evtArg.CompleteMessageAsync(msg);
            //await evtArg.RenewMessageLockAsync(msg);
            //await evtArg.AbandonMessageAsync(msg);
        };

        receiver.ProcessErrorAsync += evtArg => {
            Console.WriteLine("Ooops");
            Console.WriteLine(evtArg.Exception.Message);
            return Task.CompletedTask;
        };

        await receiver.StartProcessingAsync();
        Console.WriteLine("Press Enter to quit processing");
        Console.ReadLine();
        await receiver.StopProcessingAsync();

    }
}
