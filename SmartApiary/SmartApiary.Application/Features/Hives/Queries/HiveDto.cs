namespace SmartApiary.Application.Features.Hives.Queries
{
    public record HiveDto(
        Guid Id,
        Guid ApiaryId,
        string Label,
        string Type,
        string Color,
        int QueenAge,
        string? Note,
        string? ScaleSerialNumber
    );
}
