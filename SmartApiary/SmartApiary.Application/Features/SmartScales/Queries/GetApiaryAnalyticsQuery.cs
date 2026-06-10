using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Queries
{
    public record GetApiaryAnalyticsQuery(Guid ApiaryId, int Days = 7) : IRequest<Result<ApiaryAnalyticsDto>>;

    public class ApiaryAnalyticsDto
    {
        public List<DailyYieldDto> YieldLog { get; set; } = new();
        public List<ClimateTrendDto> ClimateLog { get; set; } = new();
    }

    // DTO za prvi grafik: Razlika u tezini (20h - 08h) za pcelinjak
    public class DailyYieldDto
    {
        public string Date { get; set; } = string.Empty; 
        public double NetYieldKg { get; set; }           // Rezultat racuna
    }

    // DTO za drugi grafik: Kretanje temp. i vazduha kroz vreme
    public class ClimateTrendDto
    {
        public string Time { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double Humidity { get; set; }
    }
}
