using MediatR;
using SmartApiary.Domain.Common;
using System.Collections.Generic;

namespace SmartApiary.Application.Features.Apiaries.Queries
{
    public record GetApiariesForUserQuery() : IRequest<Result<List<ApiaryListItemDto>>>;

    public class ApiaryListItemDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
