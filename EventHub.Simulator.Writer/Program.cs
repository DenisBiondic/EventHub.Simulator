using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using EventHub.Simulator.Shared;
using Microsoft.Azure.EventHubs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace EventHub.Simulator.Writer
{
    public static class Program
    {
        private static EventHubClient _client;

        public static void Main(string[] args)
        {
            MainAsync(args).GetAwaiter().GetResult();
        }

        private static async Task MainAsync(string[] args)
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var connectionString = configuration["SIMULATOR_EVENT_HUB_CONNECTION_STRING"];
            var connectionStringBuilder = new EventHubsConnectionStringBuilder(connectionString);

            _client = EventHubClient.CreateFromConnectionString(connectionStringBuilder.ToString());

            await SendBatchesToEventHub(1000);
            await _client.CloseAsync();
        }

        private static async Task SendBatchesToEventHub(int messagesPerBatch)
        {
            Stopwatch stopwatch = Stopwatch.StartNew();
            var random = new Random();

            var i = 0;
            while (true)
            {
                var batch = new List<EventData>();

                for (int j = 0; j < messagesPerBatch; j++)
                {
                    var message = new TestMessage
                    {
                        Value1 = random.Next(0, 100000).ToString(),
                        Value2 = $"TestMessage {i}, Some big payload: {Guid.NewGuid()}, Time: {DateTime.Now}"
                    };
                    var eventData = new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(message)));
                    batch.Add(eventData);
                }

                Console.WriteLine($"Sending batch ({messagesPerBatch} items (actually: {batch.Count})): { i }");
                await _client.SendAsync(batch);

                if (i % 20 == 0 && i != 0)
                {
                    var totalMessages = 20 * messagesPerBatch;
                    var elapsedTotalSeconds = stopwatch.Elapsed.TotalSeconds;
                    Console.WriteLine($"{totalMessages} messages sent in {elapsedTotalSeconds} seconds. " +
                                      $"Msg per sec: {totalMessages / elapsedTotalSeconds}");
                    stopwatch.Restart();
                }

                i++;
            }
        }
    }
}