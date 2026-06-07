using MediatR;
using SmartApiary.Application.Features.SmartScales.Events;
using SmartApiary.Application.Interfaces.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.EventHandlers
{
    public class SendRealtimeTelemetryHandler(IScaleNotificationService notificationService) : INotificationHandler<TelemetryReceivedEvent>
    {
        public async Task Handle(TelemetryReceivedEvent notification, CancellationToken ct)
        {
            await notificationService.SendRealtimeTelemetryAsync(notification.Telemetry);
        }
    }
}
