using Azure.Storage.Queues;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace SmartApiary.Infrastructure.Persistence.AzureQueue
{
    internal abstract class AzureQueueService<T>(
        QueueClient queueClient,
        ILogger logger) where T : class
    {
        protected readonly QueueClient _queueClient = queueClient;
        protected readonly ILogger _logger = logger;

        public async Task SendMessageAsync(T message, CancellationToken ct = default)
        {
            if (message == null) return;

            try
            {
                var json = JsonSerializer.Serialize(message);

                await _queueClient.CreateIfNotExistsAsync(cancellationToken: ct);
                await _queueClient.SendMessageAsync(json, cancellationToken: ct);

                _logger.LogInformation(
                    "Message {MessageType} sent to queue {QueueName}",
                    typeof(T).Name,
                    _queueClient.Name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Error sending message to queue {QueueName}",
                    _queueClient.Name);
            }
        }
    }
}