using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Infrastructure.Services.Email;
using System.Text.Json;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartApiary.Functions.Processing
{
    internal class ProcessActivationEmails(
        ILogger<ProcessActivationEmails> logger,
        IEmailService emailService)
    {
        [Function(nameof(ProcessActivationEmails))]
        public async Task RunAsync(
            [QueueTrigger("%AzureQueueOptions:ActivationQueue%", Connection = "AzureWebJobsStorage")]
            string message)
        {
            logger.LogInformation("ACTIVATION RAW MESSAGE: {Message}", message);

            var payload = JsonSerializer.Deserialize<ActivationMessage>(message);

            if (payload is null)
            {
                logger.LogWarning("ActivationMessage deserialization failed.");
                return;
            }

            var body =
                $"Activation token: {payload.ActivationToken}\n" +
                $"Later this should be a frontend activation link.";

            await emailService.SendEmailAsync(
                payload.Email,
                "SmartApiary - Activate account",
                body);
        }
    }
}