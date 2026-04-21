using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public record UpdateHiveLogEntryCommand(
        Guid Id,
        DateTime ReviewDate,
        string ReviewType,
        string BottomBoardColor,
        int HoneyFrames,
        double HoneyAmountKg,
        int BroodFrames,
        bool QueenPresent,
        string? Note
    ) : IRequest<Result>;
}
