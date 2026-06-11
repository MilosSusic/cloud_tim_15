using MediatR;
using Microsoft.Extensions.Logging;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.SmartScales.Commands
{
    public class SaveYieldRecordHandler : IRequestHandler<SaveYieldRecordCommand, Result>
    {
        private readonly IYieldRecordRepository _yieldRecordRepository;
        private readonly ILogger<SaveYieldRecordHandler> _logger;

        public SaveYieldRecordHandler(
            IYieldRecordRepository yieldRecordRepository,
            ILogger<SaveYieldRecordHandler> logger)
        {
            _yieldRecordRepository = yieldRecordRepository;
            _logger = logger;
        }

        public async Task<Result> Handle(SaveYieldRecordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("[YIELD HANDLER] Received command: ScaleId={ScaleId}, DeltaKg={Delta}, RecordedAt={Time}",
                request.ScaleId, request.DeltaKg, request.RecordedAt);

            var yieldRecordResult = YieldRecord.Create(
                request.ScaleId,
                request.DeltaKg,
                request.RecordedAt
            );

            if (yieldRecordResult.IsFailure)
            {
                _logger.LogWarning("[YIELD HANDLER] Failed to create yield record: {Error}", yieldRecordResult.Error);
                return Result.Failure(yieldRecordResult.Error!);
            }

            await _yieldRecordRepository.SaveAsync(yieldRecordResult.Value, cancellationToken);

            _logger.LogInformation(
                "[YIELD HANDLER] Yield record SAVED successfully! ScaleId: {ScaleId}, Delta: {Delta}kg, Time: {Time}",
                request.ScaleId,
                request.DeltaKg,
                request.RecordedAt);

            return Result.Success();
        }
    }
}
