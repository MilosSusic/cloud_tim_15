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

        public static Hive Create(
            Guid apiaryId,
            string label,
            string type,
            string color,
            int queenAge,
            string? note)
        {
            ValidateApiaryId(apiaryId);
            ValidateDetails(label, type, color, queenAge, note);

            return new Hive
            {
                Id = Guid.NewGuid(),
                ApiaryId = apiaryId,
                Label = label,
                Type = type,
                Color = color,
                QueenAge = queenAge,
                Note = note,
                IsDeleted = false
            };
        }

        public void UpdateDetails(
            string label,
            string type,
            string color,
            int queenAge,
            string? note)
        {
            ValidateDetails(label, type, color, queenAge, note);

            Label = label;
            Type = type;
            Color = color;
            QueenAge = queenAge;
            Note = note;
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        private static void ValidateApiaryId(Guid apiaryId)
        {
            if (apiaryId == Guid.Empty)
                throw new ArgumentException("Apiary id is required.", nameof(apiaryId));
        }

        private static void ValidateDetails(
            string label,
            string type,
            string color,
            int queenAge,
            string? note)
        {
            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Hive label is required.", nameof(label));
            if (label.Length > 50)
                throw new ArgumentException("Hive label must not exceed 50 characters.", nameof(label));
            if (string.IsNullOrWhiteSpace(type))
                throw new ArgumentException("Hive type is required.", nameof(type));
            if (type.Length > 50)
                throw new ArgumentException("Hive type must not exceed 50 characters.", nameof(type));
            if (string.IsNullOrWhiteSpace(color))
                throw new ArgumentException("Hive color is required.", nameof(color));
            if (color.Length > 50)
                throw new ArgumentException("Hive color must not exceed 50 characters.", nameof(color));
            if (queenAge < 0 || queenAge > 10)
                throw new ArgumentException("Queen age must be between 0 and 10.", nameof(queenAge));
            if (!string.IsNullOrWhiteSpace(note) && note.Length > 512)
                throw new ArgumentException("Note must not exceed 512 characters.", nameof(note));
        }
    }
}
