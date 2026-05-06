using MediatR;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.SpraySchedule.Queries
{
    public record GetSprayLogsQuery(
        Guid? FieldId,
        DateTime? From,
        DateTime? To
    ) : IRequest<Result<List<SprayLogDto>>>;

    public class SprayLogDto
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public string? CropName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public string? PesticideType { get; set; }
        public double? WindSpeed { get; set; }
        public double? Temperature { get; set; }
        public string? WeatherDescription { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GetSprayLogsQueryHandler
        : IRequestHandler<GetSprayLogsQuery, Result<List<SprayLogDto>>>
    {
        private readonly ISprayLogRepository _sprayLogRepository;

        public GetSprayLogsQueryHandler(ISprayLogRepository sprayLogRepository)
        {
            _sprayLogRepository = sprayLogRepository;
        }

        public async Task<Result<List<SprayLogDto>>> Handle(
            GetSprayLogsQuery request, CancellationToken cancellationToken)
        {
            var logs = await _sprayLogRepository.GetByFilterAsync(
                request.FieldId, request.From, request.To, cancellationToken);

            var dtos = logs.Select(l => new SprayLogDto
            {
                Id = l.Id,
                FieldName = l.FieldName,
                CropName = l.CropName,
                StartDateTime = l.StartDateTime,
                EndDateTime = l.EndDateTime,
                PesticideType = l.PesticideType,
                WindSpeed = l.WindSpeed,
                Temperature = l.Temperature,
                WeatherDescription = l.WeatherDescription,
                CreatedAt = l.CreatedAt
            }).ToList();

            return Result<List<SprayLogDto>>.Success(dtos);
        }
    }
}