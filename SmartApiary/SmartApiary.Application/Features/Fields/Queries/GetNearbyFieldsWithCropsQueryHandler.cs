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

            // Get all apiaries for this user
            var apiaries = await _apiaryRepository.GetByOwnerIdAsync(userId, cancellationToken);
            if (apiaries.Count == 0)
                return new List<FieldWithCropsDto>();

            // Get all fields (parcels)
            var allFields = await _fieldRepository.GetAllWithCropsAsync(cancellationToken); // This should be replaced with a method to get all fields, not just by owner
            // If not available, you may need to add a method to get all fields in IFieldRepository

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
                            Icon = GetCropIcon(c.CropName),
                            FloweringPeriod = GetFloweringPeriod(c.CropName)
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

        private static string GetCropIcon(CropType cropType)
        {
            // Return icon name or url based on crop type
            return cropType switch
            {
                CropType.Wheat => "wheat.png",
                CropType.Rice => "rice.png",
                CropType.Barley => "barley.png",
                CropType.Oats => "oats.png",
                CropType.Corn => "corn.png",
                CropType.Sorghum => "sorghum.png",
                CropType.Buckwheat => "buckwheat.png",
                CropType.Chia => "chia.png",
                _ => "crop.png"
            };
        }

        private static string GetFloweringPeriod(CropType cropType)
        {
            // Return expected flowering period for each crop
            return cropType switch
            {
                CropType.Wheat => "May - June",
                CropType.Rice => "June - July",
                CropType.Barley => "May - June",
                CropType.Oats => "May - June",
                CropType.Corn => "July - August",
                CropType.Sorghum => "July - August",
                CropType.Buckwheat => "June - July",
                CropType.Chia => "July - September",
                _ => "Unknown"
            };
        }
    }
}
