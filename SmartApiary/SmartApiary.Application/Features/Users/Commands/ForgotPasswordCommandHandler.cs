using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class ForgotPasswordCommandHandler(IUserRepository userRepository)
        : IRequestHandler<ForgotPasswordCommand, Result>
    {
        public async Task<Result> Handle(ForgotPasswordCommand request, CancellationToken ct)
        {
            var user = await userRepository.GetByEmailAsync(request.Email, ct);

            if (user is null)
            {
                return Result.Success();
            }

            user.GenerateResetPasswordToken();

            await userRepository.SaveChangesAsync(ct);

            return Result.Success();
        }
    }
}