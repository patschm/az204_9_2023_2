﻿using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace EvtHubClient;

class Program
{
    private static string conStr = "Endpoint=sb://ps-account.servicebus.windows.net/;SharedAccessKeyName=pomper;SharedAccessKey=R4lEaIRJS8JN/Xvw9DxzgCpcqFilU/Tna+AEhAI7o5E=;EntityPath=hup";
    private static string hubName = "hup";

    static async Task Main(string[] args)
    {
        await using (var producerClient = new EventHubProducerClient(conStr, hubName))
        {
            int i = 1;
            ConsoleKeyInfo key;
            do
            {
                var eventBatch = await producerClient.CreateBatchAsync();
                for (int j = 0; j < 200; j++, i++)
                {
                    eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes($"Hello World {i}")));
                }
                await producerClient.SendAsync(eventBatch);
                Console.WriteLine("Sent");
                key = Console.ReadKey();
            } while (key.Key != ConsoleKey.Escape);
        }

        Console.WriteLine("Done!");
        Console.ReadLine();
    }
}
