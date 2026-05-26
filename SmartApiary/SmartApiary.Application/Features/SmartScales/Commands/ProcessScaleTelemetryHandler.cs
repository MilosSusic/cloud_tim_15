using MediatR;
using Microsoft.Extensions.Logging;
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
    internal class ProcessScaleTelemetryHandler(
        ITelemetryRepository telemetryRepository, // Tvoj novi repozitorijum za Azure Table Storage
        ISmartScaleRepository scaleRepository,   // Postojeći SQL repozitorijum za provera tokena
        ILogger<ProcessScaleTelemetryHandler> logger
    ) : IRequestHandler<ProcessScaleTelemetryCommand, Result>
    {
        public async Task<Result> Handle(ProcessScaleTelemetryCommand request, CancellationToken ct)
        {
            // 1. Validacija tokena i pronalaženje vage u SQL bazi
            var scale = await scaleRepository.GetByTokenAsync(request.DeviceToken, ct);
            if (scale == null)
            {
                logger.LogWarning("[TELEMETRY] Failed attempt with unauthorized token.");
                return Result.Failure("Unauthorized device token.");
            }

            // 2. Kreiranje čistog domenskog modela sa tvojim Value Object-ima
            // Pošto Telemetry.Create sam unutra pravi novi EntityId.New() za merenje, prosleđujemo samo ID vage
            var telemetryResult = Telemetry.Create(
                scale.Id.ToString(), // ID vage povezuje SQL entitet i Table Storage merenje
                request.Weight,
                request.Humidity,
                request.Temperature,
                request.BatteryLevel,
                request.Timestamp
            );

            if (telemetryResult.IsFailure)
            {
                // Sklonjen .Message jer je telemetryResult.Error već string
                return Result.Failure(telemetryResult.Error!);
            }

            var telemetry = telemetryResult.Value;

            // 3. Upisivanje direktno u Azure Table Storage preko tvog repozitorijuma
            await telemetryRepository.SaveAsync(telemetry!, ct);

            logger.LogInformation("[TELEMETRY] Scale measurement successfully saved to Table Storage for Scale ID: {ScaleId}", scale.Id);

            return Result.Success();
        }
    }
}
