using SmartApiary.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartApiary.Domain.ValueObjects
{
    public sealed record EntityId
    {
        public string Value { get; }

        private EntityId(string value)
        {
            Value = value;
        }

        public static Result<EntityId> Create(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return Result<EntityId>.Failure("Id format is invalid.");

            if (!Guid.TryParse(value, out _))
                return Result<EntityId>.Failure("Invalid Id format.");

            return Result<EntityId>.Success(new EntityId(value));
        }

        public static EntityId New() => new(Guid.NewGuid().ToString());

        public override string ToString() => Value;

        public static implicit operator string(EntityId id) => id.Value;
    }
}
