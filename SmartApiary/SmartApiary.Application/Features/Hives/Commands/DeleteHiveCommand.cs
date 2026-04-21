using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Hives.Commands
{
    public record DeleteHiveCommand(Guid Id) : IRequest<Result>;
}
