namespace SmartApiary.Application.Common.Dtos
{
    public record SprayNotificationDto(
         Guid SprayScheduleId,
         Guid FieldId,
         string NotificationType,
         DateTime StartDateTime,
         int DurationHours,
         string? PesticideType,
         string To,
         string FirstName,
         string LastName,
         int NearbyApiaryCount
     );
}
