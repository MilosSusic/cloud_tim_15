using SmartApiary.Application.Interfaces.Email;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Infrastructure.Services.Email
{
    public class ScaleAlertEmailService(IEmailService emailService) : IScaleAlertEmailService
    {
        public async Task SendAnomalyAlertAsync(string toEmail, string anomalyType, string message, double currentValue)
        {
            string subject = $"Urgent SmartApiary Alert: {anomalyType}";
            var sb = new StringBuilder();
            sb.AppendLine($"<h2>Alert from your apiary!</h2>");
            sb.AppendLine($"<p><strong>Alert Type:</strong> {anomalyType}</p>");
            sb.AppendLine($"<p><strong>Details:</strong> {message}</p>");
            sb.AppendLine($"<p><strong>Recorded Value:</strong> {currentValue}</p>");
            sb.AppendLine($"<p>Please check your beehives as soon as possible.</p>");

            await emailService.SendEmailAsync(toEmail, subject, sb.ToString());
        }
    }
}
