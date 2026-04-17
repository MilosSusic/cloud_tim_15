using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace SmartApiary.Functions.Processing
{
    internal class ProcessActivationEmails(ILogger<ProcessActivationEmails> logger)
    {
        [Function(nameof(ProcessActivationEmails))]
        public Task RunAsync(
            [QueueTrigger("%AzureQueueOptions:ActivationQueue%", Connection = "AzureWebJobsStorage")]
            string message)
        {
            logger.LogInformation("ACTIVATION RAW MESSAGE: {Message}", message);

            return Task.CompletedTask;
        }
    }
}