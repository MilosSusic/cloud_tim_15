using FluentValidation;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public class UpdateHiveCommandValidator : AbstractValidator<UpdateHiveCommand>
    {
        public UpdateHiveCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hive id is required.");

            RuleFor(x => x.Label)
                .NotEmpty().WithMessage("Hive label is required.")
                .MaximumLength(50);

            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Hive type is required.")
                .MaximumLength(50);

            RuleFor(x => x.Color)
                .NotEmpty().WithMessage("Hive color is required.")
                .MaximumLength(50);

            RuleFor(x => x.QueenAge)
                .GreaterThanOrEqualTo(0).WithMessage("Queen age must be zero or greater.")
                .LessThanOrEqualTo(10).WithMessage("Queen age cannot be greater than 10 years.");

            RuleFor(x => x.Note)
                .MaximumLength(512)
                .When(x => !string.IsNullOrWhiteSpace(x.Note));
        }
    }
}
