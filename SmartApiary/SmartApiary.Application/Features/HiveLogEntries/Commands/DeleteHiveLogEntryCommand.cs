using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public record DeleteHiveLogEntryCommand(Guid Id) : IRequest<Result>;
}
