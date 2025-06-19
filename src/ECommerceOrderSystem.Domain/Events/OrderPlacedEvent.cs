using System;

namespace ECommerceOrderSystem.Domain.Events
{
    public record OrderPlacedEvent
    {
        public Guid OrderId { get; init; }
        public string UserId { get; init; }
        public string ProductId { get; init; }
        public int Quantity { get; init; }
        public string PaymentMethod { get; init; }
        public DateTime PlacedAt { get; init; }

        public OrderPlacedEvent(Guid orderId, string userId, string productId, int quantity, string paymentMethod)
        {
            OrderId = orderId;
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            Quantity = quantity;
            PaymentMethod = paymentMethod ?? throw new ArgumentNullException(nameof(paymentMethod));
            PlacedAt = DateTime.UtcNow;
        }
    }
}