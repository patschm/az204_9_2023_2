using Azure.Storage.Queues;

namespace StorageQueueWriter;

class Program
{
    static string ConnectionString = "DefaultEndpointsProtocol=https;AccountName=psqueuedemo;AccountKey=oXC76/5VcTdqUIGFh53iRH2evVx52UK0aObTYnyVnpBTz2b/w+1AV0d/W98ZTZPyfINpiJVvItM1+AStyLw9KQ==;EndpointSuffix=core.windows.net";
    static string QueueName = "myqueue";
    static async Task Main(string[] args)
    {
        await WriteToQueueAsync();
        Console.WriteLine("Press Enter to Quit");
        Console.ReadLine();
    }

    private static async Task WriteToQueueAsync()
    {
        var client = new QueueClient(ConnectionString, QueueName);
        for (int i = 0; i < 100; i++)
        {
            await client.SendMessageAsync($"Hello Number {i}");
        }
    }

}
