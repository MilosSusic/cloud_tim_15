using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class RescheduleSprayCommandValidator : AbstractValidator<RescheduleSprayCommand>
    {
        public RescheduleSprayCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Spray schedule ID is required.");
            RuleFor(x => x.StartDateTime)
                .NotEmpty().WithMessage("New start date and time is required.");
        }
    }
}
