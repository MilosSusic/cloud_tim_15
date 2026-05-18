using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Common;

namespace SmartApiary.Infrastructure.Services.Email
{
    public class SprayNotificationOrchestrator(
        ISprayNotificationRepository sprayNotificationRepository,
        ISprayNotificationEmailService sprayNotificationEmailService) : ISprayNotificationOrchestrator
    {
        public async Task<Result> ProcessAsync(SprayNotificationMessage payload, CancellationToken ct = default)
        {
            var recipientsByField = await sprayNotificationRepository
                .GetFieldRecipientsInRadiusAsync(payload.FieldId, 5.0, ct);

            if (recipientsByField is null || recipientsByField.Recipients.Count == 0)
            {
                return Result.Success();
            }

            foreach (var recipient in recipientsByField.Recipients)
            {
                if (payload.NotificationType == "Scheduled")
                {
                    await sprayNotificationEmailService.SendScheduledAsync(
                        recipient.Email,
                        recipient.FirstName,
                        recipient.LastName,
                        recipientsByField.FieldName,
                        payload.StartDateTime,
                        payload.DurationHours,
                        payload.PesticideType,
                        recipient.NearbyApiaryCount);
                }
                else if (payload.NotificationType == "Rescheduled")
                {
                    await sprayNotificationEmailService.SendRescheduledAsync(
                        recipient.Email,
                        recipient.FirstName,
                        recipient.LastName,
                        recipientsByField.FieldName,
                        payload.StartDateTime,
                        payload.DurationHours,
                        payload.PesticideType,
                        recipient.NearbyApiaryCount);
                }
                else if (payload.NotificationType == "Cancelled")
                {
                    await sprayNotificationEmailService.SendCancelledAsync(
                        recipient.Email,
                        recipient.FirstName,
                        recipient.LastName,
                        recipientsByField.FieldName,
                        payload.StartDateTime,
                        payload.DurationHours,
                        payload.PesticideType,
                        recipient.NearbyApiaryCount);
                }
            }

            return Result.Success();
        }
    }
}
