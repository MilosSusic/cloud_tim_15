using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public record ActivateAccountCommand(
        string ActivationToken,
        string Password,
        string ConfirmPassword
    ) : IRequest<Result>;
}