using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public record ProcessScaleTelemetryCommand : IRequest<Result>
    {
        public string DeviceToken { get; init; } = string.Empty;
        public double Weight { get; init; }
        public double Humidity { get; init; }
        public double Temperature { get; init; }
        public double BatteryLevel { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
