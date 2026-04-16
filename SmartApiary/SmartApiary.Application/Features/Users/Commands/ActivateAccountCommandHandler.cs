using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class ActivateAccountCommandHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
        : IRequestHandler<ActivateAccountCommand, Result>
    {
        public async Task<Result> Handle(ActivateAccountCommand request, CancellationToken ct)
        {
            if (request.Password != request.ConfirmPassword)
            {
                return Result.Failure("Passwords do not match.");
            }

            var user = await userRepository.GetByActivationTokenAsync(request.ActivationToken, ct);

            if (user is null)
            {
                return Result.Failure("Invalid activation token.");
            }

            if (user.ActivationTokenExpiresAtUtc is null || user.ActivationTokenExpiresAtUtc < DateTime.UtcNow)
            {
                return Result.Failure("Activation token has expired.");
            }

            var passwordHash = passwordHasher.Hash(request.Password);

            user.Activate(passwordHash);

            await userRepository.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}