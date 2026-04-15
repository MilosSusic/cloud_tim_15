using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Queries
{
    public class LoginQueryHandler(IUserRepository userRepository)
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

            if (user.PasswordHash != request.Password)
            {
                return Result<string>.Failure("Invalid email or password.");
            }

            return Result<string>.Success("token-placeholder");
        }
    }
}