using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class UpdateSprayScheduleCommandValidator : AbstractValidator<UpdateSprayScheduleCommand>
    {
        public UpdateSprayScheduleCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Spray schedule ID is required.");
        }
    }
}
