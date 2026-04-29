using MediatR;
using Microsoft.AspNetCore.Http;
using SmartApiary.Application.Features.Fields.Queries;
using SmartApiary.Application.Interfaces.Repositories;
using SmartApiary.Domain.Common;
using SmartApiary.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Application.Features.SpraySchedule.Queries
{
    public record GetSprayScheduleForFieldQuery(Guid FieldId) : IRequest<Result<List<SprayListItemDto>>>;

    public class SprayListItemDto
    {
        public Guid Id { get; set; }
        public Guid FieldId { get; set; }
        public DateTime StartDateTime { get; set; }
        public int DurationHours { get; set; }
        public string? PesticideType { get; set; }
        public SprayStatus Status { get; set; } 
        public DateTime CreatedAt { get; set; } 
        public DateTime? UpdatedAt { get; set; }
    }
}
