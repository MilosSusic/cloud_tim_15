using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public class UpdateApiaryCommandHandler : IRequestHandler<UpdateApiaryCommand, Result>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UpdateApiaryCommandHandler(
            IApiaryRepository apiaryRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result> Handle(UpdateApiaryCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result.Failure("Invalid user id.");

            var apiary = await _apiaryRepository.GetByIdAsync(request.Id, cancellationToken);
            if (apiary is null)
                return Result.Failure("Apiary not found.");

            if (apiary.OwnerId != userId)
                return Result.Failure("You do not have access to this apiary.");

            apiary.Name = request.Name;
            apiary.Description = request.Description;
            apiary.Latitude = request.Latitude;
            apiary.Longitude = request.Longitude;

            // Sliku menjamo samo ako je prosledjena nova (npr. nakon ponovnog upload-a u Blob)
            if (!string.IsNullOrWhiteSpace(request.ImageUrl))
                apiary.ImageUrl = request.ImageUrl;

            if (!string.IsNullOrWhiteSpace(request.ThumbnailUrl))
                apiary.ThumbnailUrl = request.ThumbnailUrl;

            _apiaryRepository.Update(apiary);
            await _apiaryRepository.SaveChangesAsync(cancellationToken);

            return Result.Success();
        }
    }
}
