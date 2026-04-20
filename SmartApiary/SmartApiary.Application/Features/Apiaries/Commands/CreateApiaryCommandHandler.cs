using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Models;

namespace SmartApiary.Application.Features.Apiaries.Commands
{
    public class CreateApiaryCommandHandler : IRequestHandler<CreateApiaryCommand, Result<Guid>>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CreateApiaryCommandHandler(IApiaryRepository apiaryRepository, IHttpContextAccessor httpContextAccessor)
        {
            _apiaryRepository = apiaryRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<Result<Guid>> Handle(CreateApiaryCommand request, CancellationToken cancellationToken)
        {
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return Result<Guid>.Failure("Invalid user id.");

            var apiary = new Apiary
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                ImageUrl = request.ImageUrl,
                ThumbnailUrl = request.ThumbnailUrl,
                OwnerId = userId // Always set to current user
            };

            await _apiaryRepository.AddAsync(apiary, cancellationToken);
            await _apiaryRepository.SaveChangesAsync(cancellationToken);

            return Result<Guid>.Success(apiary.Id);
        }
    }
}
