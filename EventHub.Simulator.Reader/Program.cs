using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.EventHubs;
using Microsoft.Azure.EventHubs.Processor;
using Microsoft.Extensions.Configuration;

namespace EventHub.Simulator.Reader
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Registering EventProcessor...");

            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();
            var configuration = builder.Build();

            var connectionString = configuration["EVENT_HUB_READER_CONNECTION_STRING"];

            // remove the entity path if given in connection string, because it should be provided over separate configuration key
            int entityPathIndex = connectionString.IndexOf("EntityPath=", StringComparison.Ordinal);

            if (entityPathIndex != -1)
                connectionString = connectionString.Substring(0, entityPathIndex);

            var eventHubPath = configuration["EVENT_HUB_READER_PATH"];
            var storageAccountName = configuration["STORAGE_ACCOUNT_NAME"];
            var storageAccountKey = configuration["STORAGE_ACCOUNT_KEY"];
            var storageContainerName = configuration["STORAGE_CONTAINER_NAME"];

            var storageConnectionString = $"DefaultEndpointsProtocol=https;AccountName={storageAccountName};" +
                          $"AccountKey={storageAccountKey}";

            var eventProcessorHost = new EventProcessorHost(
                eventHubPath,
                PartitionReceiver.DefaultConsumerGroupName,
                connectionString,
                storageConnectionString,
                storageContainerName);

            // Registers the Event Processor Host and starts receiving messages
            var eventProcessorOptions = EventProcessorOptions.DefaultOptions;
            eventProcessorOptions.MaxBatchSize = 1000;
            eventProcessorOptions.PrefetchCount = 1000;

            await eventProcessorHost.RegisterEventProcessorAsync<SimpleEventProcessor>(eventProcessorOptions);
            await Task.Delay(Timeout.Infinite);
        }
    }
}
