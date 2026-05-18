using System;

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

        public static HiveLogEntry Create(
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
            ValidateHiveId(hiveId);
            ValidateDetails(reviewDate, reviewType, bottomBoardColor, honeyFrames, honeyAmountKg, broodFrames, note);

            return new HiveLogEntry
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
            };
        }

        public void UpdateDetails(
            DateTime reviewDate,
            string reviewType,
            string bottomBoardColor,
            int honeyFrames,
            double honeyAmountKg,
            int broodFrames,
            bool queenPresent,
            string? note)
        {
            ValidateDetails(reviewDate, reviewType, bottomBoardColor, honeyFrames, honeyAmountKg, broodFrames, note);

            ReviewDate = reviewDate;
            ReviewType = reviewType;
            BottomBoardColor = bottomBoardColor;
            HoneyFrames = honeyFrames;
            HoneyAmountKg = honeyAmountKg;
            BroodFrames = broodFrames;
            QueenPresent = queenPresent;
            Note = note;
        }

        private static void ValidateHiveId(Guid hiveId)
        {
            if (hiveId == Guid.Empty)
                throw new ArgumentException("Hive id is required.", nameof(hiveId));
        }

        private static void ValidateDetails(
            DateTime reviewDate,
            string reviewType,
            string bottomBoardColor,
            int honeyFrames,
            double honeyAmountKg,
            int broodFrames,
            string? note)
        {
            if (reviewDate == default)
                throw new ArgumentException("Review date is required.", nameof(reviewDate));
            if (reviewDate > DateTime.UtcNow.AddMinutes(5))
                throw new ArgumentException("Review date cannot be in the future.", nameof(reviewDate));
            if (string.IsNullOrWhiteSpace(reviewType))
                throw new ArgumentException("Review type is required.", nameof(reviewType));
            if (reviewType.Length > 100)
                throw new ArgumentException("Review type must not exceed 100 characters.", nameof(reviewType));
            if (!string.IsNullOrWhiteSpace(bottomBoardColor) && bottomBoardColor.Length > 50)
                throw new ArgumentException("Bottom board color must not exceed 50 characters.", nameof(bottomBoardColor));
            if (honeyFrames < 0)
                throw new ArgumentException("Honey frames must be zero or greater.", nameof(honeyFrames));
            if (honeyAmountKg < 0)
                throw new ArgumentException("Honey amount must be zero or greater.", nameof(honeyAmountKg));
            if (broodFrames < 0)
                throw new ArgumentException("Brood frames must be zero or greater.", nameof(broodFrames));
            if (!string.IsNullOrWhiteSpace(note) && note.Length > 512)
                throw new ArgumentException("Note must not exceed 512 characters.", nameof(note));
        }
    }
}
