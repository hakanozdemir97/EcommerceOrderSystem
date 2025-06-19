using System;

namespace ECommerceOrderSystem.Domain.ValueObjects
{
    public record UserId
    {
        public string Value { get; }

        public UserId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("User ID cannot be null or empty", nameof(value));
            
            Value = value;
        }

        public static implicit operator string(UserId userId) => userId.Value;
        public static implicit operator UserId(string value) => new(value);
    }
}