using FluentValidation;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public class CreateApiaryCommandValidator : AbstractValidator<CreateApiaryCommand>
    {
        public CreateApiaryCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Apiary name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Description)
                .MaximumLength(512)
                .When(x => !string.IsNullOrWhiteSpace(x.Description));

            RuleFor(x => x.Latitude)
                .InclusiveBetween(-90, 90)
                .WithMessage("Latitude must be between -90 and 90.");

            RuleFor(x => x.Longitude)
                .InclusiveBetween(-180, 180)
                .WithMessage("Longitude must be between -180 and 180.");

            RuleFor(x => x.ImageUrl)
                .MaximumLength(512)
                .When(x => !string.IsNullOrWhiteSpace(x.ImageUrl));

            RuleFor(x => x.ThumbnailUrl)
                .MaximumLength(512)
                .When(x => !string.IsNullOrWhiteSpace(x.ThumbnailUrl));
        }
    }
}
