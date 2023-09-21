﻿using Azure;
using Azure.Messaging.EventGrid;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EvtGridCmds;

class Program
{
    static string topicHost = "ps-grid.westeurope-1.eventgrid.azure.net";
    static string topicEP = $"https://{topicHost}/api/events";
    static string topicKey = "bJrQIs9GH31X0OhODOeb8yuUfddBrYtwk4nWaw68IBE=";

    static async Task Main(string[] args)
    {
        var topicCredentials = new AzureKeyCredential(topicKey);
        var client = new EventGridPublisherClient(new Uri(topicEP), topicCredentials);

        int i = 0;
        var events = new List<EventGridEvent>();
        ConsoleKey key;
        do
        {
            Console.WriteLine("Firing event...");
            var evtData = new Dictionary<string, string> { { "een", $"{i++}" }, { "twee", "Hello from code" } };
            events.Clear();
            var gev = new EventGridEvent("Handmatig afgevuurd", "Click", "2.0", evtData);
            
            events.Add(gev);
            //await client.SendEventsAsync(events);
            await client.SendEventAsync(gev);
            Console.WriteLine("Another event? (Esc to quit)");
            key = Console.ReadKey().Key;
        }
        while (key != ConsoleKey.Escape);
        Console.WriteLine("Done!");
        Console.ReadLine();
    }
}
