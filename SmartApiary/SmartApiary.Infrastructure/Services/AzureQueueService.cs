using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using SmartApiary.Application.Interfaces.Messaging;
using System.Text.Json;

namespace SmartApiary.Infrastructure.Services
{
    public class AzureQueueService : IQueueService
    {
        private readonly string _connectionString;

        public AzureQueueService(IConfiguration configuration)
        {
            _connectionString = configuration["AzureQueueOptions:ConnectionString"]!;
        }

        public async Task SendAsync<T>(string queueName, T message, CancellationToken ct = default)
        {
            var client = new QueueClient(_connectionString, queueName);

            await client.CreateIfNotExistsAsync(cancellationToken: ct);

            var json = JsonSerializer.Serialize(message);

            await client.SendMessageAsync(json, cancellationToken: ct);
        }
    }
}