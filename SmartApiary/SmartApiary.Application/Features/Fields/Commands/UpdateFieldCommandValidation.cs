using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Fields.Commands
{
    public class UpdateFieldCommandValidator : AbstractValidator<UpdateFieldCommand>
    {
        public UpdateFieldCommandValidator()
        {
            RuleFor(x => x.FieldName)
                .NotEmpty().WithMessage("Field name is required.");

            RuleFor(x => x.Description)
                .MaximumLength(512)
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.Latitude)
                .NotEmpty().WithMessage("Latitude is required.")
                .InclusiveBetween(-90, 90).WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .NotEmpty().WithMessage("Longitude is required.")
                .InclusiveBetween(-180, 180).WithMessage("Longitude must be between -180 and 180.");
        }
    }
}
