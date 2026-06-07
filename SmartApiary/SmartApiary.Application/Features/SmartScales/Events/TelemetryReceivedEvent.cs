using MediatR;
using SmartApiary.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Events
{
    public record TelemetryReceivedEvent(Telemetry Telemetry) : INotification;
}
