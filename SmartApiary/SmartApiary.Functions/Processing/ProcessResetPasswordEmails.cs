using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Infrastructure.Services.Email;
using System.Text.Json;
using SmartApiary.Infrastructure.Persistence.AzureQueue.Messages;

namespace SmartApiary.Functions.Processing
{
    internal class ProcessResetPasswordEmails(
        ILogger<ProcessResetPasswordEmails> logger,
        IEmailService emailService)
    {
        [Function(nameof(ProcessResetPasswordEmails))]
        public async Task RunAsync(
            [QueueTrigger("%AzureQueueOptions:ResetPasswordQueue%", Connection = "AzureWebJobsStorage")]
            string message)
        {
            logger.LogInformation("RAW MESSAGE: {Message}", message);

            var payload = JsonSerializer.Deserialize<ResetPasswordMessage>(message);

            if (payload is null)
            {
                logger.LogWarning("ResetPasswordMessage deserialization failed.");
                return;
            }

            var body =
                $"Reset password token: {payload.ResetPasswordToken}\n" +
                $"Later this should be a frontend reset link.";

            await emailService.SendEmailAsync(
                payload.Email,
                "SmartApiary - Reset password",
                body);
        }
    }
}