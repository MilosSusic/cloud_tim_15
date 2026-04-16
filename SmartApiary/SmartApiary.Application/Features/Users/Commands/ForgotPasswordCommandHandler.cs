using MediatR;
using SmartApiary.Application.Interfaces.Messaging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public class ForgotPasswordCommandHandler(IUserRepository userRepository, IResetPasswordQueueService queueService)
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

            await queueService.SendResetPasswordAsync(
                user.Email,
                user.ResetPasswordToken!,
                ct);

            return Result.Success();
        }
    }
}