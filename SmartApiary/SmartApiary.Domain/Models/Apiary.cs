using System;
using System.Collections.Generic;

namespace SmartApiary.Domain.Models
{
    public class Apiary
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string? ImageUrl { get; set; }
        public string? ThumbnailUrl { get; set; }
        public Guid OwnerId { get; set; }
        public List<Hive> Hives { get; set; } = new();
    }
}

