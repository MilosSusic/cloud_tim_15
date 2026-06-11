using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Queries
{
    public class GetApiaryAnalyticsQueryHandler : IRequestHandler<GetApiaryAnalyticsQuery, Result<ApiaryAnalyticsDto>>
    {
        private readonly ISmartScaleRepository _scaleRepository;
        private readonly ITelemetryRepository _telemetryRepository;
        private readonly IYieldRecordRepository _yieldRecordRepository;

        public GetApiaryAnalyticsQueryHandler(
            ISmartScaleRepository scaleRepository,
            ITelemetryRepository telemetryRepository,
            IYieldRecordRepository yieldRecordRepository)
        {
            _scaleRepository = scaleRepository;
            _telemetryRepository = telemetryRepository;
            _yieldRecordRepository = yieldRecordRepository;
        }

        public async Task<Result<ApiaryAnalyticsDto>> Handle(GetApiaryAnalyticsQuery request, CancellationToken cancellationToken)
        {
            // Pronalazimo vagu povezanu sa ovim pcelinjakom
            var scale = await _scaleRepository.GetByApiaryIdAsync(request.ApiaryId, cancellationToken);
            if (scale == null)
            {
                return Result<ApiaryAnalyticsDto>.Failure("No active smart scale found for this apiary.");
            }

            var dto = new ApiaryAnalyticsDto();

            // --- GRAFIK 1: ČITAJ IZ YIELDRECORDS TABELE ---
            var startDate = DateTime.UtcNow.Date.AddDays(-request.Days);
            var yieldRecords = await _yieldRecordRepository.GetHistoryAsync(scale.Id.ToString(), startDate, cancellationToken);

            // Mapiranje u DTO
            dto.YieldLog = yieldRecords
                .OrderBy(r => r.RecordedAt)
                .Select(r => new DailyYieldDto
                {
                    Date = r.RecordedAt.ToLocalTime().ToString("HH:mm:ss"),
                    Delta = r.Delta
                })
                .ToList();

            // --- GRAFIK 2: KLIMA (Temperatura i vlažnost) ---
            var telemetryHistory = (await _telemetryRepository.GetHistoryAsync(scale.Id.ToString(), startDate, cancellationToken)).ToList();

            dto.ClimateLog = telemetryHistory
                .OrderBy(t => t.Timestamp)
                .Select(t => new ClimateTrendDto
                {
                    Time = t.Timestamp.ToLocalTime().ToString("HH:mm:ss"), 
                    Temperature = Math.Round(t.Temperature.Value, 1),
                    Humidity = Math.Round(t.Humidity.Value, 1)
                })
                .ToList();

            // --- LATEST TELEMETRY ---
            if (telemetryHistory.Count > 0)
            {
                var latest = telemetryHistory.Last();
                dto.LatestTelemetry = new
                {
                    weight = latest.Weight.Value,
                    temperature = latest.Temperature.Value,
                    humidity = latest.Humidity.Value,
                    batteryLevel = latest.BatteryLevel.Value,
                    timestamp = latest.Timestamp
                };
            }

            return Result<ApiaryAnalyticsDto>.Success(dto);
        }
    }
}
