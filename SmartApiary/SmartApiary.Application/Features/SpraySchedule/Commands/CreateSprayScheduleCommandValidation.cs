using FluentValidation;
using SmartApiary.Application.Features.Fields.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class CreateSprayScheduleCommandValidator : AbstractValidator<CreateSprayScheduleCommand>
    {
        public CreateSprayScheduleCommandValidator()
        {
            RuleFor(x => x.FieldId)
                .NotEmpty().WithMessage("Field ID is required.");

            RuleFor(x => x.StartDateTime)
                .NotEmpty().WithMessage("Start date and time is required.");

            RuleFor(x => x.DurationHours)
                .GreaterThan(0).WithMessage("Duration must be greater than 0.");

            RuleFor(x => x.PesticideType)
                .MaximumLength(256)
                .When(x => !string.IsNullOrEmpty(x.PesticideType));
        }
    }
}
