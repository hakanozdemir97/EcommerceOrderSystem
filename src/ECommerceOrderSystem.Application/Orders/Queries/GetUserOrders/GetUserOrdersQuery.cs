using System;
using System.Collections.Generic;
using MediatR;
using ECommerceOrderSystem.Application.Common.Models;

namespace ECommerceOrderSystem.Application.Orders.Queries.GetUserOrders
{
    public record GetUserOrdersQuery : IRequest<Result<IEnumerable<UserOrderResponse>>>
    {
        public string UserId { get; init; } = string.Empty;
    }

    public record UserOrderResponse
    {
        public Guid OrderId { get; init; }
        public string ProductId { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
        public DateTime? ProcessedAt { get; init; }
    }
}