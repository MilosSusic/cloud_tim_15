using System;
using System.Collections.Generic;

namespace SmartApiary.Domain.Models
{
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
}
