using System;
using SmartApiary.Domain.Common;

namespace SmartApiary.Domain.Models
{
    public class HiveLogEntry
    {
        public Guid Id { get; private set; }
        public Guid HiveId { get; private set; }
        public DateTime ReviewDate { get; private set; }
        public string ReviewType { get; private set; } = string.Empty;
        public string BottomBoardColor { get; private set; } = string.Empty;
        public int HoneyFrames { get; private set; }
        public double HoneyAmountKg { get; private set; }
        public int BroodFrames { get; private set; }
        public bool QueenPresent { get; private set; }
        public string? Note { get; private set; }

        public static Result<HiveLogEntry> Create(
            Guid hiveId,
            DateTime reviewDate,
            string reviewType,
            string bottomBoardColor,
            int honeyFrames,
            double honeyAmountKg,
            int broodFrames,
            bool queenPresent,
            string? note)
        {
            var hiveValidation = ValidateHiveId(hiveId);
            if (hiveValidation.IsFailure)
                return Result<HiveLogEntry>.Failure(hiveValidation.Error!);

            var detailsValidation = ValidateDetails(reviewDate, reviewType, bottomBoardColor, honeyFrames, honeyAmountKg, broodFrames, note);
            if (detailsValidation.IsFailure)
                return Result<HiveLogEntry>.Failure(detailsValidation.Error!);

            return Result<HiveLogEntry>.Success(new HiveLogEntry
            {
                Id = Guid.NewGuid(),
                HiveId = hiveId,
                ReviewDate = reviewDate,
                ReviewType = reviewType,
                BottomBoardColor = bottomBoardColor,
                HoneyFrames = honeyFrames,
                HoneyAmountKg = honeyAmountKg,
                BroodFrames = broodFrames,
                QueenPresent = queenPresent,
                Note = note
            });
        }

        public Result UpdateDetails(
            DateTime reviewDate,
            string reviewType,
            string bottomBoardColor,
            int honeyFrames,
            double honeyAmountKg,
            int broodFrames,
            bool queenPresent,
            string? note)
        {
            var detailsValidation = ValidateDetails(reviewDate, reviewType, bottomBoardColor, honeyFrames, honeyAmountKg, broodFrames, note);
            if (detailsValidation.IsFailure)
                return detailsValidation;

            ReviewDate = reviewDate;
            ReviewType = reviewType;
            BottomBoardColor = bottomBoardColor;
            HoneyFrames = honeyFrames;
            HoneyAmountKg = honeyAmountKg;
            BroodFrames = broodFrames;
            QueenPresent = queenPresent;
            Note = note;

            return Result.Success();
        }

        private static Result ValidateHiveId(Guid hiveId)
        {
            if (hiveId == Guid.Empty)
                return Result.Failure("Hive id is required.");

            return Result.Success();
        }

        private static Result ValidateDetails(
            DateTime reviewDate,
            string reviewType,
            string bottomBoardColor,
            int honeyFrames,
            double honeyAmountKg,
            int broodFrames,
            string? note)
        {
            if (reviewDate == default)
                return Result.Failure("Review date is required.");
            if (reviewDate > DateTime.UtcNow.AddMinutes(5))
                return Result.Failure("Review date cannot be in the future.");
            if (string.IsNullOrWhiteSpace(reviewType))
                return Result.Failure("Review type is required.");
            if (reviewType.Length > 100)
                return Result.Failure("Review type must not exceed 100 characters.");
            if (!string.IsNullOrWhiteSpace(bottomBoardColor) && bottomBoardColor.Length > 50)
                return Result.Failure("Bottom board color must not exceed 50 characters.");
            if (honeyFrames < 0)
                return Result.Failure("Honey frames must be zero or greater.");
            if (honeyAmountKg < 0)
                return Result.Failure("Honey amount must be zero or greater.");
            if (broodFrames < 0)
                return Result.Failure("Brood frames must be zero or greater.");
            if (!string.IsNullOrWhiteSpace(note) && note.Length > 512)
                return Result.Failure("Note must not exceed 512 characters.");

            return Result.Success();
        }
    }
}
