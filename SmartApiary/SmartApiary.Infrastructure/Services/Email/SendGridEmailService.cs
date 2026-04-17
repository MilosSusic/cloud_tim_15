using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace SmartApiary.Infrastructure.Services.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string? _apiKey;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(
            IConfiguration configuration,
            ILogger<SendGridEmailService> logger)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body)
        {
            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                _logger.LogInformation(
                    "[MOCK EMAIL] To: {To}, Subject: {Subject}, Body: {Body}",
                    to, subject, body);
                return;
            }

            var client = new SendGridClient(_apiKey);

            var from = new EmailAddress("test@smartapiary.com", "SmartApiary");
            var toEmail = new EmailAddress(to);

            var msg = MailHelper.CreateSingleEmail(
                from,
                toEmail,
                subject,
                plainTextContent: body,
                htmlContent: body);

            var response = await client.SendEmailAsync(msg);

            _logger.LogInformation(
                "SendGrid email sent to {To}. Status code: {StatusCode}",
                to,
                response.StatusCode);
        }
    }
}