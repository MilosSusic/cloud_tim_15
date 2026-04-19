using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Email;
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

            var resetLink = $"http://localhost:5173/reset-password?token={payload.ResetPasswordToken}";

            var body = $@"
                <h3>SmartApiary - Resetovanje lozinke</h3>
                <p>Primili smo zahtev za resetovanje lozinke za Vaš nalog.</p>
                <p>Kliknite na dugme ispod kako biste postavili novu lozinku:</p>
                <p><a href=""{resetLink}"" style=""padding: 10px 20px; background-color: #e65100; color: white; text-decoration: none; border-radius: 5px; display: inline-block;"">Resetuj lozinku</a></p>
                <br/>
                <p>Ukoliko dugme ne radi, iskopirajte ovaj link u Vaš pretraživač:</p>
                <p>{resetLink}</p>
                <br/>
                <p>Ako niste zahtevali resetovanje lozinke, možete ignorisati ovaj mejl.</p>
                <br/>
                <p>SmartApiary Tim</p>
            ";

            await emailService.SendEmailAsync(
                payload.Email,
                "SmartApiary - Reset password",
                body);
        }
    }
}