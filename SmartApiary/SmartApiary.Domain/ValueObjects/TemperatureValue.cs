using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.ValueObjects
{
    public sealed record TemperatureValue : IComparable<TemperatureValue>
    {
        public double Value { get; }

        private TemperatureValue(double value)
        {
            Value = value;
        }

        public static Result<TemperatureValue> Create(double value)
        {
            if (value < -40 || value > 60)
                return Result<TemperatureValue>.Failure("Temperature must be between -40°C and 60°C.");

            return Result<TemperatureValue>.Success(new TemperatureValue(value));
        }

        public int CompareTo(TemperatureValue? other) =>
            other is null ? 1 : Value.CompareTo(other.Value);

        public static bool operator >(TemperatureValue left, TemperatureValue right) => left.Value > right.Value;
        public static bool operator <(TemperatureValue left, TemperatureValue right) => left.Value < right.Value;
        public static bool operator >=(TemperatureValue left, TemperatureValue right) => left.Value >= right.Value;
        public static bool operator <=(TemperatureValue left, TemperatureValue right) => left.Value <= right.Value;

        public static implicit operator double(TemperatureValue temp) => temp.Value;

        public override string ToString() => $"{Value} °C";
    }
}
