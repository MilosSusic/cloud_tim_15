using FluentValidation;
using SmartApiary.Domain.Enums;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("First name is required.")
                .MaximumLength(100);

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Last name is required.")
                .MaximumLength(100);

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Role)
                .Must(role => Enum.IsDefined(typeof(UserRole), role))
                .WithMessage("Invalid role.");

            RuleFor(x => x.PhoneNumber)
                .MaximumLength(32)
                .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
        }
    }
}