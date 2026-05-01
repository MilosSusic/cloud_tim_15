using SmartApiary.Application.Interfaces.Email;
using System.Text;

namespace SmartApiary.Infrastructure.Services.Email
{
    public class SprayNotificationEmailService(IEmailService emailService) : ISprayNotificationEmailService
    {
        public async Task SendScheduledAsync(string to, string firstName, string lastName, string fieldName, DateTime startDateTime, int durationHours, string? pesticideType, int nearbyApiaryCount)
        {
            var (subject, body) = BuildEmail(
                "Scheduled",
                firstName,
                lastName,
                fieldName,
                startDateTime,
                durationHours,
                pesticideType,
                nearbyApiaryCount);

            await emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendRescheduledAsync(string to, string firstName, string lastName, string fieldName, DateTime startDateTime, int durationHours, string? pesticideType, int nearbyApiaryCount)
        {
            var (subject, body) = BuildEmail(
                "Rescheduled",
                firstName,
                lastName,
                fieldName,
                startDateTime,
                durationHours,
                pesticideType,
                nearbyApiaryCount);

            await emailService.SendEmailAsync(to, subject, body);
        }

        public async Task SendCancelledAsync(string to, string firstName, string lastName, string fieldName, DateTime startDateTime, int durationHours, string? pesticideType, int nearbyApiaryCount)
        {
            var (subject, body) = BuildEmail(
                "Cancelled",
                firstName,
                lastName,
                fieldName,
                startDateTime,
                durationHours,
                pesticideType,
                nearbyApiaryCount);

            await emailService.SendEmailAsync(to, subject, body);
        }

        private static (string Subject, string Body) BuildEmail(
            string notificationType,
            string firstName,
            string lastName,
            string fieldName,
            DateTime startDateTime,
            int durationHours,
            string? pesticideType,
            int nearbyApiaryCount)
        {
            var date = startDateTime.ToString("dd.MM.yyyy HH:mm");
            var pesticide = string.IsNullOrWhiteSpace(pesticideType) ? "N/A" : pesticideType;

            var actionText = notificationType switch
            {
                "Scheduled" => "A pesticide treatment has been scheduled",
                "Rescheduled" => "A pesticide treatment has been rescheduled",
                "Cancelled" => "A pesticide treatment has been cancelled",
                _ => "A pesticide treatment has been updated"
            };

            var subject = notificationType switch
            {
                "Scheduled" => "SmartApiary - Pesticide treatment scheduled nearby",
                "Rescheduled" => "SmartApiary - Pesticide treatment rescheduled nearby",
                "Cancelled" => "SmartApiary - Pesticide treatment cancelled nearby",
                _ => "SmartApiary - Pesticide treatment update"
            };

            var sb = new StringBuilder();
            sb.AppendLine($"<h3>Hello {firstName} {lastName},</h3>");
            sb.AppendLine($"<p>{actionText} near your apiary/apiaries.</p>");
            sb.AppendLine($"<p><strong>Field:</strong> {fieldName}</p>");
            sb.AppendLine($"<p><strong>Planned start:</strong> {date}</p>");
            sb.AppendLine($"<p><strong>Duration:</strong> {durationHours}h</p>");
            sb.AppendLine($"<p><strong>Pesticide:</strong> {pesticide}</p>");
            sb.AppendLine($"<p><strong>Your nearby apiaries in radius:</strong> {nearbyApiaryCount}</p>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<p>SmartApiary Team</p>");

            return (subject, sb.ToString());
        }
    }
}
