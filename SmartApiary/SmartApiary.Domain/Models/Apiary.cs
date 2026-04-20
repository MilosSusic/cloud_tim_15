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

    public class Hive
    {
        public Guid Id { get; set; }
        public Guid ApiaryId { get; set; }
        public string Label { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public int QueenAge { get; set; }
        public string? Note { get; set; }
        public bool IsDeleted { get; set; }
        public List<HiveLogEntry> LogEntries { get; set; } = new();
    }

    public class HiveLogEntry
    {
        public Guid Id { get; set; }
        public Guid HiveId { get; set; }
        public DateTime ReviewDate { get; set; }
        public string ReviewType { get; set; } = string.Empty;
        public string BottomBoardColor { get; set; } = string.Empty;
        public int HoneyFrames { get; set; }
        public double HoneyAmountKg { get; set; }
        public int BroodFrames { get; set; }
        public bool QueenPresent { get; set; }
        public string? Note { get; set; }
    }
}
