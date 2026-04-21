using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.HiveLogEntries.Commands
{
    public class CreateHiveLogEntryCommandHandler : IRequestHandler<CreateHiveLogEntryCommand, Result<Guid>>
    {
        private readonly IHiveLogEntryRepository _logRepository;
        private readonly IHiveRepository _hiveRepository;
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateHiveLogEntryCommandHandler(
            IHiveLogEntryRepository logRepository,
            IHiveRepository hiveRepository,
            IApiaryRepository apiaryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _logRepository = logRepository;
            _hiveRepository = hiveRepository;
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Guid>> Handle(CreateHiveLogEntryCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<Guid>.Failure("Invalid user id.");

            var hive = await _hiveRepository.GetByIdAsync(request.HiveId, cancellationToken);
            if (hive is null)
                return Result<Guid>.Failure("Hive not found.");

            var apiary = await _apiaryRepository.GetByIdAsync(hive.ApiaryId, cancellationToken);
            if (apiary is null || apiary.OwnerId != userId)
                return Result<Guid>.Failure("You do not have access to this hive.");

            var entry = new HiveLogEntry
            {
                Id = Guid.NewGuid(),
                HiveId = request.HiveId,
                ReviewDate = request.ReviewDate,
                ReviewType = request.ReviewType,
                BottomBoardColor = request.BottomBoardColor,
                HoneyFrames = request.HoneyFrames,
                HoneyAmountKg = request.HoneyAmountKg,
                BroodFrames = request.BroodFrames,
                QueenPresent = request.QueenPresent,
                Note = request.Note
            };

            await _logRepository.AddAsync(entry, cancellationToken);
            await _logRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(entry.Id);
        }
    }
}
