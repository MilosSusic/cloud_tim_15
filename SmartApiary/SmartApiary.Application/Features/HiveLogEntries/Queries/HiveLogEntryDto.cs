namespace SmartApiary.Application.Features.HiveLogEntries.Queries
{
    public record HiveLogEntryDto(
        Guid Id,
        Guid HiveId,
        DateTime ReviewDate,
        string ReviewType,
        string BottomBoardColor,
        int HoneyFrames,
        double HoneyAmountKg,
        int BroodFrames,
        bool QueenPresent,
        string? Note
    );
}
