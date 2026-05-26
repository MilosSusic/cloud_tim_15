using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.ValueObjects
{
    public sealed record Weight : IComparable<Weight>
    {
        public double Value { get; }

        private Weight(double value)
        {
            Value = value;
        }

        public static Result<Weight> Create(double value)
        {
            if (value < 0)
                return Result<Weight>.Failure("Weight cannot be negative.");

            return Result<Weight>.Success(new Weight(value));
        }

        public int CompareTo(Weight? other) =>
            other is null ? 1 : Value.CompareTo(other.Value);

        public static bool operator >(Weight left, Weight right) => left.Value > right.Value;
        public static bool operator <(Weight left, Weight right) => left.Value < right.Value;
        public static bool operator >=(Weight left, Weight right) => left.Value >= right.Value;
        public static bool operator <=(Weight left, Weight right) => left.Value <= right.Value;

        // Omogućava da pise: double w = weightInstance;
        public static implicit operator double(Weight weight) => weight.Value;

        public override string ToString() => $"{Value} kg";
    }
}
