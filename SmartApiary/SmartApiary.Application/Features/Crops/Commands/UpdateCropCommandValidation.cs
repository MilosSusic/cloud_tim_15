using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Crops.Commands
{
    public class UpdateCropCommandValidator : AbstractValidator<UpdateCropCommand>
    {
        public UpdateCropCommandValidator()
        {
            RuleFor(x => x.CropName)
                .NotEmpty().WithMessage("Crop name is required.");

            RuleFor(x => x.Description)
                .MaximumLength(512)
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.FloweringPeriodStart)
                 .NotEmpty().WithMessage("Flowering period start is required.");

            RuleFor(x => x.FloweringPeriodEnd)
                 .NotEmpty().WithMessage("Flowering period end is required.")
                 .GreaterThanOrEqualTo(x => x.FloweringPeriodStart).WithMessage("Flowering period end must be after the start date.");
        }
    }
}
