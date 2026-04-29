using MediatR;
using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.Fields.Queries
{
    public record GetFieldsForUserQuery() : IRequest<Result<List<FieldListItemDto>>>;

    public class FieldListItemDto
    {
        public Guid Id { get; set; }
        public string FieldName { get; set; } = string.Empty;
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
    }
}
