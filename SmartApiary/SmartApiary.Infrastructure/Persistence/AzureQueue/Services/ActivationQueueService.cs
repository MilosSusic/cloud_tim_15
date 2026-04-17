using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Services
{
    internal class ActivationQueueService : IActivationQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<ActivationQueueService> _logger;

        public ActivationQueueService(
            IConfiguration configuration,
            ILogger<ActivationQueueService> logger)
        {
            _logger = logger;

            var connectionString = configuration["AzureQueueOptions:ConnectionString"]
                ?? throw new InvalidOperationException("AzureQueueOptions:ConnectionString is not configured.");

            var queueName = configuration["AzureQueueOptions:ActivationQueue"]
                ?? throw new InvalidOperationException("AzureQueueOptions:ActivationQueue is not configured.");

            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            _queueClient = new QueueClient(connectionString, queueName, options);
        }

        public async Task SendActivationAsync(string email, string activationToken, CancellationToken ct = default)
        {
            var message = new ActivationMessage
            {
                Email = email,
                ActivationToken = activationToken
            };

            var json = System.Text.Json.JsonSerializer.Serialize(message);

            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);
            await _queueClient.SendMessageAsync(json, cancellationToken: ct);

            _logger.LogInformation(
                "ActivationMessage sent to queue {QueueName} for {Email}",
                _queueClient.Name,
                email);
        }
    }
}