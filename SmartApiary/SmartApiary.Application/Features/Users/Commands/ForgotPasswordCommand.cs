using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Users.Commands
{
    public record ForgotPasswordCommand(string Email) : IRequest<Result>;
}