using MediatR;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;

namespace SmartApiary.Application.Features.SpraySchedule.Queries
{
    public record GetSprayScheduleForFieldQuery(Guid FieldId) : IRequest<Result<List<SprayListItemDto>>>;

    public class SprayListItemDto
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int DurationHours { get; set; }
        public string? PesticideType { get; set; }
        public SprayStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
    }
}
