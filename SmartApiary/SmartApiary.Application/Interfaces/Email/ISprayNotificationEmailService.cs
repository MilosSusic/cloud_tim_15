using SmartApiary.Application.Common.Dtos;

namespace SmartApiary.Application.Interfaces.Email
{
    public interface ISprayNotificationEmailService
    {
       
            Task SendScheduledAsync(SprayNotificationDto dto);
            Task SendRescheduledAsync(SprayNotificationDto dto);
            Task SendCancelledAsync(SprayNotificationDto dto);
        
    }
}
