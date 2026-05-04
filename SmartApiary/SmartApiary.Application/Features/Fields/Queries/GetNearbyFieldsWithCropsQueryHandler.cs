using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Fields.Queries;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Enums;
using SmartApiary.Domain.Models;
using System.Security.Claims;

namespace SmartApiary.Application.Features.Fields.Queries
{
    public class GetNearbyFieldsWithCropsQueryHandler : IRequestHandler<GetNearbyFieldsWithCropsQuery, List<FieldWithCropsDto>>
    {
        private readonly IApiaryRepository _apiaryRepository;
        private readonly IFieldRepository _fieldRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetNearbyFieldsWithCropsQueryHandler(
            IApiaryRepository apiaryRepository,
            IFieldRepository fieldRepository,
            IUserRepository userRepository,
            IHttpContextAccessor httpContextAccessor)
        {
            _apiaryRepository = apiaryRepository;
            _fieldRepository = fieldRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<FieldWithCropsDto>> Handle(GetNearbyFieldsWithCropsQuery request, CancellationToken cancellationToken)
        {
            // Get current user id from context
            var userIdStr = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!Guid.TryParse(userIdStr, out var userId))
                return new List<FieldWithCropsDto>();

            
            var apiaries = await _apiaryRepository.GetByOwnerIdAsync(userId, cancellationToken);
            if (apiaries.Count == 0)
                return new List<FieldWithCropsDto>();

            
            var allFields = await _fieldRepository.GetAllWithCropsAsync(cancellationToken); 

            var result = new List<FieldWithCropsDto>();
            foreach (var field in allFields)
            {
                // Check if field is within 5km of any apiary
                foreach (var apiary in apiaries)
                {
                    if (GetDistanceKm(apiary.Latitude, apiary.Longitude, field.Latitude, field.Longitude) <= 5.0)
                    {
                        // Get owner info
                        var owner = await _userRepository.GetByIdAsync(field.OwnerId, cancellationToken);
                        var ownerDto = new OwnerDto
                        {
                            FirstName = owner?.FirstName ?? string.Empty,
                            LastName = owner?.LastName ?? string.Empty,
                            Email = owner?.Email ?? string.Empty,
                            Phone = owner?.PhoneNumber ?? string.Empty
                        };
                        // Map crops
                        var crops = field.Crops.Select(c => new CropDto
                        {
                            Name = c.CropName.ToString(),
                            Icon = field.ThumbnailUrl ?? field.ImageUrl ?? string.Empty,
                            FloweringPeriodStart = c.FloweringPeriodStart,
                            FloweringPeriodEnd = c.FloweringPeriodEnd,
                            FloweringPeriod = $"{c.FloweringPeriodStart:yyyy-MM-dd} - {c.FloweringPeriodEnd:yyyy-MM-dd}"
                        }).ToList();
                        result.Add(new FieldWithCropsDto
                        {
                            FieldId = field.Id,
                            FieldName = field.FieldName,
                            Latitude = field.Latitude,
                            Longitude = field.Longitude,
                            Area = 0, // Add area if available
                            Owner = ownerDto,
                            Crops = crops
                        });
                        break;
                    }
                }
            }
            return result;
        }

        private static double GetDistanceKm(double lat1, double lon1, double lat2, double lon2)
        {
            // Haversine formula
            var R = 6371.0;
            var dLat = DegreesToRadians(lat2 - lat1);
            var dLon = DegreesToRadians(lon2 - lon1);
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(DegreesToRadians(lat1)) * Math.Cos(DegreesToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        private static double DegreesToRadians(double deg) => deg * (Math.PI / 180.0);
    }
}
