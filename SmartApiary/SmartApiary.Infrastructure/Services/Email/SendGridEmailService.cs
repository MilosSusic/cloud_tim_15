using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using SmartApiary.Application.Interfaces.Email;

namespace SmartApiary.Infrastructure.Services.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string? _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;
        private readonly ILogger<SendGridEmailService> _logger;

        public SendGridEmailService(
            IConfiguration configuration,
            ILogger<SendGridEmailService> logger)
        {
            _apiKey = configuration["SendGrid:ApiKey"];
            _fromEmail = configuration["SendGrid:FromEmail"] ?? "test@smartapiary.com";
            _fromName = configuration["SendGrid:FromName"] ?? "SmartApiary";
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

            var from = new EmailAddress(_fromEmail, _fromName);
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