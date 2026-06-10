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

        public GetApiaryAnalyticsQueryHandler(
            ISmartScaleRepository scaleRepository,
            ITelemetryRepository telemetryRepository)
        {
            _scaleRepository = scaleRepository;
            _telemetryRepository = telemetryRepository;
        }

        public async Task<Result<ApiaryAnalyticsDto>> Handle(GetApiaryAnalyticsQuery request, CancellationToken cancellationToken)
        {
            // Pronalazimo vagu povezanu sa ovim pcelinjakom
            var scale = await _scaleRepository.GetByApiaryIdAsync(request.ApiaryId, cancellationToken);
            if (scale == null)
            {
                return Result<ApiaryAnalyticsDto>.Failure("No active smart scale found for this apiary.");
            }

            // Povlacimo istoriju telemetrije za zadati broj dana unazad
            var startDate = DateTime.UtcNow.Date.AddDays(-request.Days);
            var telemetryHistory = (await _telemetryRepository.GetHistoryAsync(scale.Id.ToString(), startDate, cancellationToken)).ToList();

            var dto = new ApiaryAnalyticsDto();

            // --- GRAFIK 1: RACUNANJE PRINOSA NA SVAKIH 5 MINUTA ---
            // Grupisemo merenja u blokove od po 5 minuta na osnovu realnog vremena upisa
            var groupedBy5Min = telemetryHistory
                .GroupBy(t => new
                {
                    Date = t.Timestamp.ToLocalTime().Date,
                    Hour = t.Timestamp.ToLocalTime().Hour,
                    Minute = (t.Timestamp.ToLocalTime().Minute / 5) * 5
                })
                .OrderBy(g => g.Key.Date)
                .ThenBy(g => g.Key.Hour)
                .ThenBy(g => g.Key.Minute);

            foreach (var group in groupedBy5Min)
            {
                // Uzimamo prvo i poslednje merenje koja su upisana u tom petominutnom prozoru
                var firstTelemetry = group.OrderBy(t => t.Timestamp).FirstOrDefault();
                var lastTelemetry = group.OrderByDescending(t => t.Timestamp).FirstOrDefault();

                // Racunamo razliku samo ako imamo bar dva razlicita merenja unutar tih 5 minuta
                if (firstTelemetry != null && lastTelemetry != null && firstTelemetry.Id != lastTelemetry.Id)
                {
                    double netYield = lastTelemetry.Weight.Value - firstTelemetry.Weight.Value;

                    dto.YieldLog.Add(new DailyYieldDto
                    {
                        // X osa prikazuje tacan sat i minut tog intervala
                        Date = $"{group.Key.Hour:D2}:{group.Key.Minute:D2}",
                        NetYieldKg = Math.Round(netYield, 2)
                    });
                }
            }

            // --- GRAFIK 2: KLIMA (Temperatura i vlaznost) ---
            // Prikazuje sve tacke hronoloski kako pristizu
            dto.ClimateLog = telemetryHistory
                .Select(t => new ClimateTrendDto
                {
                    Time = t.Timestamp.ToLocalTime().ToString("HH:mm:ss"), 
                    Temperature = Math.Round(t.Temperature.Value, 1),
                    Humidity = Math.Round(t.Humidity.Value, 1)
                })
                .ToList();

            return Result<ApiaryAnalyticsDto>.Success(dto);

            // GRAFIK 1: RACUNANJE DNEVNOG PRINOSA (Tezina u 20h - Tezina u 08h) ---
            // Grupisemo merenja po danima
            //var groupedByDay = telemetryHistory
            //    .GroupBy(t => t.Timestamp.Date)
            //    .OrderBy(g => g.Key);

            //foreach (var dayGroup in groupedByDay)
            //{
            //    // Trazimo merenje najblize jutarnjem merenju u 8h i vecernjem merenju u 20h
            //    // Dozvoljavamo odstupanje jer ne znamo kad tacno merenje dolazi
            //    var morningTelemetry = dayGroup
            //        .Where(t => t.Timestamp.Hour >= 7 && t.Timestamp.Hour <= 9)
            //        .OrderBy(t => Math.Abs((t.Timestamp - t.Timestamp.Date.AddHours(8)).Ticks))
            //        .FirstOrDefault();

            //    var eveningTelemetry = dayGroup
            //        .Where(t => t.Timestamp.Hour >= 19 && t.Timestamp.Hour <= 21)
            //        .OrderBy(t => Math.Abs((t.Timestamp - t.Timestamp.Date.AddHours(20)).Ticks))
            //        .FirstOrDefault();

            //    // Prinos racunamo samo ako za taj dan imamo i jutarnje i vecernje merenje
            //    if (morningTelemetry != null && eveningTelemetry != null)
            //    {
            //        double netYield = eveningTelemetry.Weight.Value - morningTelemetry.Weight.Value;

            //        dto.YieldLog.Add(new DailyYieldDto
            //        {
            //            Date = dayGroup.Key.ToString("dd.MM."),
            //            NetYieldKg = Math.Round(netYield, 2)
            //        });
            //    }
            //}

            // GRAFIK 2: Temperatura i Vlaznost ---
            // Ovde samo mapiramo vremensku seriju podataka hronoloski
            //dto.ClimateLog = telemetryHistory
            //    .Select(t => new ClimateTrendDto
            //    {
            //        Time = request.Days <= 1
            //            ? t.Timestamp.ToLocalTime().ToString("HH:mm")
            //            : t.Timestamp.ToLocalTime().ToString("dd.MM. HH:mm"),
            //        Temperature = Math.Round(t.Temperature.Value, 1),
            //        Humidity = Math.Round(t.Humidity.Value, 1)
            //    })
            //    .ToList();

            //return Result<ApiaryAnalyticsDto>.Success(dto);
        }
    }
}
