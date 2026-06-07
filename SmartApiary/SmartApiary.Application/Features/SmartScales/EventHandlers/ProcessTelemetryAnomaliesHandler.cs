using MediatR;
using SmartApiary.Application.Features.SmartScales.Events;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.EventHandlers
{
    public class ProcessTelemetryAnomaliesHandler(
    IMediator mediator,
    ITelemetryRepository telemetryRepository,
    ISmartScaleRepository scaleRepository) : INotificationHandler<TelemetryReceivedEvent>
    {
        public async Task Handle(TelemetryReceivedEvent notification, CancellationToken ct)
        {
            var current = notification.Telemetry;
            var scaleIdGuid = Guid.Parse(current.SmartScaleId.ToString());

            var scale = await scaleRepository.GetByHiveIdAsync(scaleIdGuid, ct);
            if (scale == null) scale = await scaleRepository.GetBySerialNumberAsync(current.SmartScaleId.ToString(), ct);
            if (scale == null) return;

            double activeThreshold = scale.WeightDropLimit ?? SmartScaleLimits.DefaultWeightDropLimit;

            var previous = await telemetryRepository.GetLatestBeforeAsync(current.SmartScaleId.ToString(), current.Timestamp, ct);
            if (previous != null)
            {
                double weightDifference = previous.Weight.Value - current.Weight.Value;

                if (weightDifference >= activeThreshold)
                {
                    await mediator.Publish(new ScaleAnomalyDetectedEvent(
                        scale.Id,
                        "CriticalWeightDrop",
                        $"A sudden weight decrease of {weightDifference}kg has been detected. Potential hive theft or tipping.",
                        current.Weight.Value), ct);
                }
            }

            if (current.BatteryLevel.Value <= SmartScaleLimits.LowBatteryThreshold)
            {
                await mediator.Publish(new ScaleAnomalyDetectedEvent(
                    scale.Id,
                    "LowBattery",
                    $"Scale battery is critically low: {current.BatteryLevel.Value}%. Replacement required.",
                    current.BatteryLevel.Value), ct);
            }
        }
    }
}
