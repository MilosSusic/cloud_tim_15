using MediatR;
using SmartApiary.Application.Interfaces.Authentication;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Application.Interfaces.Security;
using SmartApiary.Application.Interfaces.Services;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Queries
{
    public class LoginQueryHandler(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator jwtTokenGenerator,
        IStartupScaleSimulator startupScaleSimulator)
        : IRequestHandler<LoginQuery, Result<string>>
    {
        public async Task<Result<string>> Handle(LoginQuery request, CancellationToken ct)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null)
            {
                return Result<string>.Failure("Invalid email or password.");
            }

            if (!user.IsActive)
            {
                return Result<string>.Failure("Account is not activated.");
            }

            if (user.IsSuspended)
            {
                return Result<string>.Failure("Account is not available.");
            }

            if (!passwordHasher.Verify(request.Password, user.PasswordHash))
            {
                return Result<string>.Failure("Invalid email or password.");
            }

            await startupScaleSimulator.WarmUpScalesForUserAsync(user.Id, ct);

            var token = jwtTokenGenerator.GenerateToken(user);

            return Result<string>.Success(token);
        }
    }
}