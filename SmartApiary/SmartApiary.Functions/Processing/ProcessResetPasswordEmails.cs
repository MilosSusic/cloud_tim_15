using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SmartApiary.Functions.Processing
{
    internal class ProcessResetPasswordEmails(ILogger<ProcessResetPasswordEmails> logger)
    {
        [Function(nameof(ProcessResetPasswordEmails))]
        public Task RunAsync(
            [QueueTrigger("%AzureQueueOptions:ResetPasswordQueue%", Connection = "AzureWebJobsStorage")]
            string message)
        {
            logger.LogInformation("RAW MESSAGE: {Message}", message);

            return Task.CompletedTask;
        }
    }
}