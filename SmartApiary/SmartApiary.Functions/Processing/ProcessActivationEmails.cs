using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Email;
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

            var activationLink = $"http://localhost:5173/activate?token={payload.ActivationToken}";

            var body = $@"
                <h3>Dobrodošli u SmartApiary!</h3>
                <p>Kako biste aktivirali Vaš nalog, molimo Vas da kliknete na link ispod:</p>
                <p><a href=""{activationLink}"" style=""padding: 10px 20px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; display: inline-block;"">Aktiviraj nalog</a></p>
                <br/>
                <p>Ukoliko dugme ne radi, iskopirajte ovaj link u Vaš pretraživač:</p>
                <p>{activationLink}</p>
                <br/>
                <p>SmartApiary Tim</p>
            ";

            await emailService.SendEmailAsync(
                payload.Email,
                "SmartApiary - Activate account",
                body);
        }
    }
}