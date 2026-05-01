using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Messaging;
using System.Text.Json;

namespace SmartApiary.Infrastructure.Persistence.AzureQueue.Services
{
    internal class SprayNotificationQueueService : ISprayNotificationQueueService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<SprayNotificationQueueService> _logger;

        public SprayNotificationQueueService(
            IConfiguration configuration,
            ILogger<SprayNotificationQueueService> logger)
        {
            _logger = logger;

            var connectionString = configuration["AzureQueueOptions:ConnectionString"]
                ?? throw new InvalidOperationException("AzureQueueOptions:ConnectionString is not configured.");

            var queueName = configuration["AzureQueueOptions:SprayNotificationQueue"]
                ?? throw new InvalidOperationException("AzureQueueOptions:SprayNotificationQueue is not configured.");

            var options = new QueueClientOptions
            {
                MessageEncoding = QueueMessageEncoding.Base64
            };

            _queueClient = new QueueClient(connectionString, queueName, options);
        }

        public async Task SendAsync(SprayNotificationMessage message, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(message);

            await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);
            await _queueClient.SendMessageAsync(json, cancellationToken: ct);

            _logger.LogInformation(
                "Spray notification message sent to queue {QueueName} for spray {SprayScheduleId} ({NotificationType})",
                _queueClient.Name,
                message.SprayScheduleId,
                message.NotificationType);
        }
    }
}
