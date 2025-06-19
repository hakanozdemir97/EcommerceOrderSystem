using System;
using ECommerceOrderSystem.Domain.Common;
using ECommerceOrderSystem.Domain.Enums;
using ECommerceOrderSystem.Domain.ValueObjects;

namespace ECommerceOrderSystem.Domain.Entities
{
    public class Order : BaseEntity
    {
        public UserId UserId { get; private set; }
        public ProductId ProductId { get; private set; }
        public int Quantity { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public OrderStatus Status { get; private set; }
        public DateTime? ProcessedAt { get; private set; }

        // EF Core için parameterless constructor
        private Order() { }

        public Order(UserId userId, ProductId productId, int quantity, PaymentMethod paymentMethod)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            ProductId = productId ?? throw new ArgumentNullException(nameof(productId));
            Quantity = quantity;
            PaymentMethod = paymentMethod;
            Status = OrderStatus.Pending;
        }

        public void MarkAsProcessing()
        {
            if (Status != OrderStatus.Pending)
                throw new InvalidOperationException($"Cannot mark order as processing. Current status: {Status}");

            Status = OrderStatus.Processing;
            SetUpdatedAt();
        }

        public void MarkAsCompleted()
        {
            if (Status != OrderStatus.Processing)
                throw new InvalidOperationException($"Cannot mark order as completed. Current status: {Status}");

            Status = OrderStatus.Completed;
            ProcessedAt = DateTime.UtcNow;
            SetUpdatedAt();
        }

        public void MarkAsFailed()
        {
            if (Status == OrderStatus.Completed)
                throw new InvalidOperationException("Cannot mark completed order as failed");

            Status = OrderStatus.Failed;
            SetUpdatedAt();
        }
    }
}