using System;
using System.Text;
using System.Threading.Tasks;

// New
using NewNS = Azure.Messaging.EventHubs;


namespace EvtHubClient
{
    class Program
    {
        private static string conStr = "Endpoint=sb://ps-account.servicebus.windows.net/;SharedAccessKeyName=pomper;SharedAccessKey=R4lEaIRJS8JN/Xvw9DxzgCpcqFilU/Tna+AEhAI7o5E=;EntityPath=hup";
        private static string hubName = "hup";

        static async Task Main(string[] args)
        {
            // Check!! AZ-204 book describes EventHubClient which is an obsolete package
            // Use this solutuin
            await NewStyle();

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        private static async Task NewStyle()
        {
            await using (var producerClient = new NewNS.Producer.EventHubProducerClient(conStr, hubName))
            {
                int i = 1;
                ConsoleKeyInfo key;
                do
                {
                    var eventBatch = await producerClient.CreateBatchAsync();
                    for (int j = 0; j < 200; j++, i++)
                    {
                        eventBatch.TryAdd(new NewNS.EventData(Encoding.UTF8.GetBytes($"Hello World {i}")));
                    }
                    await producerClient.SendAsync(eventBatch);
                    Console.WriteLine("Sent");
                    key = Console.ReadKey();
                } while (key.Key != ConsoleKey.Escape);
            }
        }
    }
}
