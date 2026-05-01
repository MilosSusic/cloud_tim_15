using Microsoft.Azure.Functions.Worker;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Domain.Enums;
using SmartApiary.Infrastructure.Persistence;
using System.Text;
using System.Text.Json;

namespace SmartApiary.Functions.Processing
{
    internal class ProcessSprayNotifications(
        ILogger<ProcessSprayNotifications> logger,
        IEmailService emailService,
        ApplicationDbContext dbContext)
    {
        [Function(nameof(ProcessSprayNotifications))]
        public async Task RunAsync(
            [QueueTrigger("%AzureQueueOptions:SprayNotificationQueue%", Connection = "AzureWebJobsStorage")]
            string message)
        {
            logger.LogInformation("SPRAY RAW MESSAGE: {Message}", message);

            var payload = JsonSerializer.Deserialize<SprayNotificationMessage>(message);
            if (payload is null)
            {
                logger.LogWarning("SprayNotificationMessage deserialization failed.");
                return;
            }

            var field = await dbContext.Fields
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == payload.FieldId);

            if (field is null)
            {
                logger.LogWarning("Field {FieldId} not found for spray notification.", payload.FieldId);
                return;
            }

            var allApiaries = await dbContext.Apiaries
                .AsNoTracking()
                .Select(a => new { a.OwnerId, a.Name, a.Latitude, a.Longitude })
                .ToListAsync();

            var nearbyApiaries = allApiaries
                .Where(a => GetDistanceKm(a.Latitude, a.Longitude, field.Latitude, field.Longitude) <= 5.0)
                .Select(a => new { a.OwnerId, a.Name })
                .ToList();

            if (nearbyApiaries.Count == 0)
            {
                logger.LogInformation("No apiaries found in 5km radius for field {FieldId}.", payload.FieldId);
                return;
            }

            var apiaryCountByOwner = nearbyApiaries
                .GroupBy(x => x.OwnerId)
                .ToDictionary(g => g.Key, g => g.Count());

            var ownerIds = apiaryCountByOwner.Keys.ToList();

            var recipients = await dbContext.Users
                .AsNoTracking()
                .Where(u => ownerIds.Contains(u.Id) && u.Role == UserRole.Beekeeper && u.IsActive && !u.IsSuspended)
                .Select(u => new { u.Id, u.FirstName, u.LastName, u.Email })
                .ToListAsync();

            foreach (var recipient in recipients)
            {
                var nearbyCount = apiaryCountByOwner.TryGetValue(recipient.Id, out var count) ? count : 0;
                var (subject, body) = BuildEmail(payload, field.FieldName, recipient.FirstName, recipient.LastName, nearbyCount);
                await emailService.SendEmailAsync(recipient.Email, subject, body);
            }

            logger.LogInformation(
                "Spray notification ({NotificationType}) sent to {RecipientCount} beekeepers for field {FieldId}.",
                payload.NotificationType,
                recipients.Count,
                payload.FieldId);
        }

        private static (string Subject, string Body) BuildEmail(
            SprayNotificationMessage payload,
            string fieldName,
            string firstName,
            string lastName,
            int nearbyApiaryCount)
        {
            var date = payload.StartDateTime.ToString("dd.MM.yyyy HH:mm");
            var pesticide = string.IsNullOrWhiteSpace(payload.PesticideType) ? "N/A" : payload.PesticideType;

            var actionText = payload.NotificationType switch
            {
                "Scheduled" => "A pesticide treatment has been scheduled",
                "Rescheduled" => "A pesticide treatment has been rescheduled",
                "Cancelled" => "A pesticide treatment has been cancelled",
                _ => "A pesticide treatment has been updated"
            };

            var subject = payload.NotificationType switch
            {
                "Scheduled" => "SmartApiary - Pesticide treatment scheduled nearby",
                "Rescheduled" => "SmartApiary - Pesticide treatment rescheduled nearby",
                "Cancelled" => "SmartApiary - Pesticide treatment cancelled nearby",
                _ => "SmartApiary - Pesticide treatment update"
            };

            var sb = new StringBuilder();
            sb.AppendLine($"<h3>Hello {firstName} {lastName},</h3>");
            sb.AppendLine($"<p>{actionText} near your apiary/apairies.</p>");
            sb.AppendLine($"<p><strong>Field:</strong> {fieldName}</p>");
            sb.AppendLine($"<p><strong>Planned start:</strong> {date}</p>");
            sb.AppendLine($"<p><strong>Duration:</strong> {payload.DurationHours}h</p>");
            sb.AppendLine($"<p><strong>Pesticide:</strong> {pesticide}</p>");
            sb.AppendLine($"<p><strong>Your nearby apiaries in radius:</strong> {nearbyApiaryCount}</p>");
            sb.AppendLine("<br/>");
            sb.AppendLine("<p>SmartApiary Team</p>");

            return (subject, sb.ToString());
        }

        private static double GetDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371.0;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180.0);
    }
}
