using MediatR;
using SmartApiary.Application.Features.SmartScales.Events;
using SmartApiary.Application.Interfaces.Email;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.EventHandlers
{
    public class ScaleAnomalyDetectedHandler(
    IScaleAlertEmailService emailService,
    IScaleNotificationService notificationService,
    IUserRepository userRepository) : INotificationHandler<ScaleAnomalyDetectedEvent>
    {
        public async Task Handle(ScaleAnomalyDetectedEvent notification, CancellationToken ct)
        {
            await notificationService.SendAnomalyNotificationAsync(notification.ScaleId, notification.AnomalyType, notification.Message);

            var owner = await userRepository.GetOwnerByScaleIdAsync(notification.ScaleId, ct);

            if (owner != null && !string.IsNullOrEmpty(owner.Email))
            {
                await emailService.SendAnomalyAlertAsync(
                    owner.Email,
                    notification.AnomalyType,
                    notification.Message,
                    notification.CurrentValue);
            }
        }
    }
}
