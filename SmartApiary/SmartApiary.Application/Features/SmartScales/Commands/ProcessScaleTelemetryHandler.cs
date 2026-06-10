using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Features.SmartScales.Events;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class ProcessScaleTelemetryHandler(
        ITelemetryRepository telemetryRepository, 
        ISmartScaleRepository scaleRepository,   
        ILogger<ProcessScaleTelemetryHandler> logger,
        IMediator mediator
    ) : IRequestHandler<ProcessScaleTelemetryCommand, Result>
    {
        public async Task<Result> Handle(ProcessScaleTelemetryCommand request, CancellationToken ct)
        {
            var scale = await scaleRepository.GetByTokenAsync(request.DeviceToken, ct);
            if (scale == null)
            {
                logger.LogWarning("[TELEMETRY] Failed attempt with unauthorized token.");
                return Result.Failure("Unauthorized device token.");
            }

            Guid apiaryId = await scaleRepository.GetApiaryIdByHiveIdAsync(scale.HiveId, ct);
            logger.LogInformation("[SIGNALR ROUTING] Scale HiveId: {HiveId} mapped to ApiaryId: {ApiaryId}", scale.HiveId, apiaryId);

            var telemetryResult = Telemetry.Create(
                scale.Id.ToString(),
                request.Weight,
                request.Humidity,
                request.Temperature,
                request.BatteryLevel,
                request.Timestamp
            );

            if (telemetryResult.IsFailure)
            {
                return Result.Failure(telemetryResult.Error!);
            }

            var telemetry = telemetryResult.Value;

            await telemetryRepository.SaveAsync(telemetry!, ct);

            logger.LogInformation("[TELEMETRY] Scale measurement successfully saved to Table Storage for Scale ID: {ScaleId}", scale.Id);
            
            await mediator.Publish(new TelemetryReceivedEvent(telemetry, apiaryId), ct);
            return Result.Success();
        }
    }
}
