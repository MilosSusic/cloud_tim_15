using MediatR;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class SaveYieldRecordCommand : IRequest<Result>
    {
        public string ScaleId { get; set; } = string.Empty;
        public double DeltaKg { get; set; }
        public DateTime RecordedAt { get; set; }
    }
}
