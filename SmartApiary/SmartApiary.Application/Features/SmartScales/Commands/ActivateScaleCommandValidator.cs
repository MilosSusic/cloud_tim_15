using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class ActivateScaleCommandValidator : AbstractValidator<ActivateScaleCommand>
    {
        public ActivateScaleCommandValidator()
        {
            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required for activation.")
                .Matches(@"^SA-\d{4}-\d{5}$").WithMessage("Invalid serial number format.");

            RuleFor(x => x.HardwareId)
                .NotEmpty().WithMessage("Hardware ID (UUID) is required.");
        }
    }
}
