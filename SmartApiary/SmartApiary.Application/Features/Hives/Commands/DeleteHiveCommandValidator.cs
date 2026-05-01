using FluentValidation;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public class DeleteHiveCommandValidator : AbstractValidator<DeleteHiveCommand>
    {
        public DeleteHiveCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hive id is required.");
        }
    }
}
