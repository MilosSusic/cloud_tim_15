using MediatR;
using System;
using System.Collections.Generic;

namespace SmartApiary.Application.Features.Fields.Queries
{
    public record GetNearbyFieldsWithCropsQuery(Guid UserId) : IRequest<List<FieldWithCropsDto>>;

    public class FieldWithCropsDto
    {
        public Guid FieldId { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Area { get; set; }
        public OwnerDto Owner { get; set; } = new();
        public List<CropDto> Crops { get; set; } = new();
    }

    public class OwnerDto
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
    }

    public class CropDto
    {
        public string Name { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string FloweringPeriod { get; set; } = string.Empty;
    }
}
