using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Services;
using System.Text.Json;

namespace SmartApiary.Functions.Processing
{
    internal class ProcessSprayNotifications(
        ILogger<ProcessSprayNotifications> logger,
        ISprayNotificationOrchestrator sprayNotificationOrchestrator)
    {
        [Function(nameof(ProcessSprayNotifications))]
        public async Task RunAsync(
            [QueueTrigger("%AzureQueueOptions:SprayNotificationQueue%", Connection = "AzureWebJobsStorage")]
            string message)
        {
            logger.LogInformation("SPRAY RAW MESSAGE: {Message}", message);

            var payload = JsonSerializer.Deserialize<SprayNotificationMessage>(message);
            if (payload is null)
            {
                logger.LogWarning("SprayNotificationMessage deserialization failed.");
                return;
            }

            var result = await sprayNotificationOrchestrator.ProcessAsync(payload);
            if (result.IsFailure)
            {
                logger.LogWarning("Processing spray notification failed: {Error}", result.Error);
                return;
            }

            logger.LogInformation(
                "Spray notification ({NotificationType}) processed for field {FieldId}.",
                payload.NotificationType,
                payload.FieldId);
        }
    }
}
