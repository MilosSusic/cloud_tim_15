using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class ProcessScaleTelemetryValidator : AbstractValidator<ProcessScaleTelemetryCommand>
    {
        public ProcessScaleTelemetryValidator()
        {
            RuleFor(t => t.DeviceToken)
                .NotEmpty().WithMessage("Device token is required.");

            RuleFor(t => t.Weight)
                .GreaterThanOrEqualTo(0).WithMessage("Weight cannot be negative.");

            RuleFor(t => t.BatteryLevel)
                .InclusiveBetween(0, 100).WithMessage("Battery must be between 0 and 100.");

            RuleFor(t => t.Timestamp)
                .NotEmpty()
                .LessThanOrEqualTo(DateTime.UtcNow).WithMessage("Timestamp cannot be in the future.");
        }
    }
}
