using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public record ResetPasswordCommand(
        string ResetPasswordToken,
        string Password,
        string ConfirmPassword) : IRequest<Result>;
}