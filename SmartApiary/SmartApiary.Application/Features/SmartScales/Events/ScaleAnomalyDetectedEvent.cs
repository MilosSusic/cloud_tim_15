using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Events
{
    public record ScaleAnomalyDetectedEvent(
    Guid ApiaryId,
    Guid ScaleId,
    string AnomalyType,
    string Message,
    double CurrentValue) : INotification;
}
