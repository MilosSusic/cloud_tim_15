using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Commands
{
    public class ChangeSprayStatusCommandValidator : AbstractValidator<ChangeSprayStatusCommand>
    {
        public ChangeSprayStatusCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Spray schedule ID is required.");
            RuleFor(x => x.NewStatus)
                .NotEmpty().WithMessage("New status is required.");
        }
    }
}
