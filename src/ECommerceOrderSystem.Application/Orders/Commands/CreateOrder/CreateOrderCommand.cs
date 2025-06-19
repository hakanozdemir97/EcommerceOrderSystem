using MediatR;
using ECommerceOrderSystem.Application.Common.Models;

namespace ECommerceOrderSystem.Application.Orders.Commands.CreateOrder
{
    public record CreateOrderCommand : IRequest<Result<CreateOrderResponse>>
    {
        public string UserId { get; init; } = string.Empty;
        public string ProductId { get; init; } = string.Empty;
        public int Quantity { get; init; }
        public string PaymentMethod { get; init; } = string.Empty;
    }

    public record CreateOrderResponse
    {
        public Guid OrderId { get; init; }
        public string Status { get; init; } = string.Empty;
        public DateTime CreatedAt { get; init; }
    }
}