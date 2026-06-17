using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Common.Dtos;
using System.Text;

namespace SmartApiary.Infrastructure.Services.Email
{
    public class SprayNotificationEmailService(IEmailService emailService) : ISprayNotificationEmailService
    {
        public async Task SendScheduledAsync(SprayNotificationDto dto)
        {
            var (subject, body) = BuildEmail("Scheduled", dto);
            await emailService.SendEmailAsync(dto.To, subject, body);
        }

        public async Task SendRescheduledAsync(SprayNotificationDto dto)
        {
            var (subject, body) = BuildEmail("Rescheduled", dto);
            await emailService.SendEmailAsync(dto.To, subject, body);
        }

        public async Task SendCancelledAsync(SprayNotificationDto dto)
        {
            var (subject, body) = BuildEmail("Cancelled", dto);
            await emailService.SendEmailAsync(dto.To, subject, body);
        }

        private static (string Subject, string Body) BuildEmail(string notificationType, SprayNotificationDto dto)
        {
            var date = dto.StartDateTime.ToString("dd.MM.yyyy HH:mm");
            var pesticide = string.IsNullOrWhiteSpace(dto.PesticideType) ? "N/A" : dto.PesticideType;

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
            sb.AppendLine($"<h3>Hello {dto.FirstName} {dto.LastName},</h3>");
            sb.AppendLine($"<p>{actionText} near your apiary/apiaries.</p>");
            sb.AppendLine($"<p><strong>Field:</strong> {dto.FieldId}</p>");
            sb.AppendLine($"<p><strong>Planned start:</strong> {date}</p>");
            sb.AppendLine($"<p><strong>Duration:</strong> {dto.DurationHours}h</p>");
            sb.AppendLine($"<p><strong>Pesticide:</strong> {pesticide}</p>");
            sb.AppendLine($"<p><strong>Your nearby apiaries in radius:</strong> {dto.NearbyApiaryCount}</p>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<p>SmartApiary Team</p>");

            return (subject, sb.ToString());
        }
    }
}
