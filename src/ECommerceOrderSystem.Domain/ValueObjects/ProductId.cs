using System;

namespace ECommerceOrderSystem.Domain.ValueObjects
{
    public record ProductId
    {
        public string Value { get; }

        public ProductId(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Product ID cannot be null or empty", nameof(value));
            
            Value = value;
        }

        public static implicit operator string(ProductId productId) => productId.Value;
        public static implicit operator ProductId(string value) => new(value);
    }
}