using System;
using System.Collections.Generic;
using SmartApiary.Domain.Common;

namespace SmartApiary.Domain.Models
{
    public class Hive
    {
        public Guid Id { get; private set; }
        public Guid ApiaryId { get; private set; }
        public string Label { get; private set; } = string.Empty;
        public string Type { get; private set; } = string.Empty;
        public string Color { get; private set; } = string.Empty;
        public int QueenAge { get; private set; }
        public string? Note { get; private set; }
        public bool IsDeleted { get; private set; }
        public List<HiveLogEntry> LogEntries { get; private set; } = new();

        public static Result<Hive> Create(
            Guid apiaryId,
            string label,
            string type,
            string color,
            int queenAge,
            string? note)
        {
            var apiaryValidation = ValidateApiaryId(apiaryId);
            if (apiaryValidation.IsFailure)
                return Result<Hive>.Failure(apiaryValidation.Error!);

            var detailsValidation = ValidateDetails(label, type, color, queenAge, note);
            if (detailsValidation.IsFailure)
                return Result<Hive>.Failure(detailsValidation.Error!);

            return Result<Hive>.Success(new Hive
            {
                Id = Guid.NewGuid(),
                ApiaryId = apiaryId,
                Label = label,
                Type = type,
                Color = color,
                QueenAge = queenAge,
                Note = note,
                IsDeleted = false
            });
        }

        public Result UpdateDetails(
            string label,
            string type,
            string color,
            int queenAge,
            string? note)
        {
            var detailsValidation = ValidateDetails(label, type, color, queenAge, note);
            if (detailsValidation.IsFailure)
                return detailsValidation;

            Label = label;
            Type = type;
            Color = color;
            QueenAge = queenAge;
            Note = note;

            return Result.Success();
        }

        public void MarkAsDeleted()
        {
            IsDeleted = true;
        }

        private static Result ValidateApiaryId(Guid apiaryId)
        {
            if (apiaryId == Guid.Empty)
                return Result.Failure("Apiary id is required.");

            return Result.Success();
        }

        private static Result ValidateDetails(
            string label,
            string type,
            string color,
            int queenAge,
            string? note)
        {
            if (string.IsNullOrWhiteSpace(label))
                return Result.Failure("Hive label is required.");
            if (label.Length > 50)
                return Result.Failure("Hive label must not exceed 50 characters.");
            if (string.IsNullOrWhiteSpace(type))
                return Result.Failure("Hive type is required.");
            if (type.Length > 50)
                return Result.Failure("Hive type must not exceed 50 characters.");
            if (string.IsNullOrWhiteSpace(color))
                return Result.Failure("Hive color is required.");
            if (color.Length > 50)
                return Result.Failure("Hive color must not exceed 50 characters.");
            if (queenAge < 0 || queenAge > 10)
                return Result.Failure("Queen age must be between 0 and 10.");
            if (!string.IsNullOrWhiteSpace(note) && note.Length > 512)
                return Result.Failure("Note must not exceed 512 characters.");

            return Result.Success();
        }
    }
}
