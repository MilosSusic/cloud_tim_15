using FluentValidation;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public class CreateHiveLogEntryCommandValidator : AbstractValidator<CreateHiveLogEntryCommand>
    {
        public CreateHiveLogEntryCommandValidator()
        {
            RuleFor(x => x.HiveId)
                .NotEmpty().WithMessage("Hive id is required.");

            RuleFor(x => x.ReviewDate)
                .NotEmpty().WithMessage("Review date is required.")
                .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5))
                .WithMessage("Review date cannot be in the future.");

            RuleFor(x => x.ReviewType)
                .NotEmpty().WithMessage("Review type is required.")
                .MaximumLength(100);

            RuleFor(x => x.BottomBoardColor)
                .MaximumLength(50);

            RuleFor(x => x.HoneyFrames)
                .GreaterThanOrEqualTo(0).WithMessage("Honey frames must be zero or greater.");

            RuleFor(x => x.HoneyAmountKg)
                .GreaterThanOrEqualTo(0).WithMessage("Honey amount must be zero or greater.");

            RuleFor(x => x.BroodFrames)
                .GreaterThanOrEqualTo(0).WithMessage("Brood frames must be zero or greater.");

            RuleFor(x => x.Note)
                .MaximumLength(512)
                .When(x => !string.IsNullOrEmpty(x.Note));
        }
    }
}
