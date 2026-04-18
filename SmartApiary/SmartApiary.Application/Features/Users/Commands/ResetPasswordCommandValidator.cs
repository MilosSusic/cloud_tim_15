using FluentValidation;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
    {
        public ResetPasswordCommandValidator()
        {
            RuleFor(x => x.ResetPasswordToken)
                .NotEmpty().WithMessage("Reset password token is required.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Password is required.")
                .MinimumLength(6).WithMessage("Password must be at least 6 characters.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Confirm password is required.");

            RuleFor(x => x)
                .Must(x => x.Password == x.ConfirmPassword)
                .WithMessage("Passwords do not match.");
        }
    }
}