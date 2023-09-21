using Azure.Storage.Queues;

namespace StorageQueueReader;

class Program
{
    static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psqueuedemo;AccountKey=oXC76/5VcTdqUIGFh53iRH2evVx52UK0aObTYnyVnpBTz2b/w+1AV0d/W98ZTZPyfINpiJVvItM1+AStyLw9KQ==;EndpointSuffix=core.windows.net";
    static string QueueName = "myqueue";
    static async Task Main(string[] args)
    {
        await ReadFromQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task ReadFromQueueAsync()
    {
        var client = new QueueClient(ConnectionString, QueueName);
        int i = 0;
        do
        {
            // 10 seconds "lease" time
            i++;
            var response = await client.ReceiveMessageAsync(TimeSpan.FromSeconds(30));
            if (response.Value == null)
            {
                await Task.Delay(100);
                continue;
            }
            if (i % 10 == 0)
            {
                Console.WriteLine("Oooops");
                continue;
            }
            var msg = response.Value;
            Console.WriteLine(msg.Body.ToString());

            // We need more time to process
            //await client.UpdateMessageAsync(msg.MessageId, msg.PopReceipt, msg.Body, TimeSpan.FromSeconds(30));
            // Don't forget to remove
            await client.DeleteMessageAsync(msg.MessageId, msg.PopReceipt);
        }
        while (true);
    }
}
