using System;

namespace SmartApiary.Domain.Models
{
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
