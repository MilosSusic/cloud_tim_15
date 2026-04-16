using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class ResetPasswordCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
        : IRequestHandler<ResetPasswordCommand, Result>
    {
        public async Task<Result> Handle(ResetPasswordCommand request, CancellationToken ct)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return Result.Failure("Passwords do not match.");
            }

            var user = await userRepository.GetByResetPasswordTokenAsync(request.ResetPasswordToken, ct);

            if (user is null)
            {
                return Result.Failure("Invalid reset token.");
            }

            if (user.ResetPasswordTokenExpiresAtUtc is null ||
                user.ResetPasswordTokenExpiresAtUtc < DateTime.UtcNow)
            {
                return Result.Failure("Reset token expired.");
            }

            var passwordHash = passwordHasher.Hash(request.Password);

            user.ResetPassword(passwordHash);

            await userRepository.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}