using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.SmartScales.Events;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Constants;

namespace SmartApiary.Application.Features.SmartScales.EventHandlers
{
    public class ProcessTelemetryAnomaliesHandler(
    IMediator mediator,
    ITelemetryRepository telemetryRepository,
    ISmartScaleRepository scaleRepository,
    ILogger<ProcessTelemetryAnomaliesHandler> logger) : INotificationHandler<TelemetryReceivedEvent>
    {
        public async Task Handle(TelemetryReceivedEvent notification, CancellationToken ct)
        {
            var current = notification.Telemetry;
            if (!Guid.TryParse(current.SmartScaleId.ToString(), out Guid scaleIdGuid))
            {
                logger.LogWarning("[ANOMALIES HANDLER] SmartScaleId '{Id}' is not in valid GUID format. I'm skipping anomalies.", current.SmartScaleId);
                return;
            }

            var scale = await scaleRepository.GetByIdAsync(scaleIdGuid, ct);

            if (scale == null)
            {
                logger.LogWarning("[ANOMALIES HANDLER] The scale with ID {Id} was not found in the database.", scaleIdGuid);
                return;
            }

            Guid apiaryId = await scaleRepository.GetApiaryIdByHiveIdAsync(scale.HiveId, ct);

            if (apiaryId == Guid.Empty)
            {
                logger.LogWarning("[ANOMALIES HANDLER] No apiary found for hive {HiveId}.", scale.HiveId);
                return;
            }

            double activeThreshold = scale.WeightDropLimit ?? SmartScaleLimits.DefaultWeightDropLimit;
            var previous = await telemetryRepository.GetLatestBeforeAsync(current, ct);
            if (previous != null && previous.Weight != null && current.Weight != null)
            {
                double weightDifference = previous.Weight.Value - current.Weight.Value;

                if (weightDifference >= activeThreshold)
                {
                    logger.LogWarning("[ANOMALIES HANDLER] WEIGHT DROP DETECTED! Difference: {Diff}kg", weightDifference);
                    await mediator.Publish(new ScaleAnomalyDetectedEvent(
                        apiaryId,
                        scale.Id,
                        "CriticalWeightDrop",
                        $"A sudden weight decrease of {weightDifference}kg has been detected. Potential hive theft or tipping.",
                        current.Weight.Value), ct);
                }
            }

            if (current.BatteryLevel != null && current.BatteryLevel.Value <= SmartScaleLimits.LowBatteryThreshold)
            {
                bool isFirstTimeLow = previous == null ||
                        previous.BatteryLevel == null ||
                        previous.BatteryLevel.Value > SmartScaleLimits.LowBatteryThreshold;

                if (isFirstTimeLow)
                {
                    logger.LogWarning("[ANOMALIES HANDLER] DETECTED BATTERY DROP FOR THE FIRST TIME! Current level: {BatteryLevel}%", current.BatteryLevel.Value);

                    await mediator.Publish(new ScaleAnomalyDetectedEvent(
                        apiaryId,
                        scale.Id,
                        "LowBattery",
                        $"Scale battery is critically low: {current.BatteryLevel.Value}%. Replacement required.",
                        current.BatteryLevel.Value), ct);
                }
                else
                {
                    logger.LogInformation("[ANOMALIES HANDLER] The battery is low ({BatteryLevel}%), a notification has already been sent before.", current.BatteryLevel.Value);
                }
            }
        }
    }
}
