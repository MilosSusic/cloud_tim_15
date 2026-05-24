using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class RegisterScaleCommandValidator : AbstractValidator<RegisterScaleCommand>
    {
        public RegisterScaleCommandValidator()
        {
            RuleFor(x => x.HiveId)
                .NotEmpty().WithMessage("Hive id is required.");

            RuleFor(x => x.SerialNumber)
                .NotEmpty().WithMessage("Serial number is required.")
                .Matches(@"^SA-\d{4}-\d{5}$")
                .WithMessage("Serial number must be in format SA-YYYY-XXXXX (e.g., SA-2026-12345).");
        }
    }
}
