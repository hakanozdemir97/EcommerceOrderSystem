using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using FluentValidation;
using ECommerceOrderSystem.Application.Common.Interfaces;
using ECommerceOrderSystem.Application.Common.Models;
using ECommerceOrderSystem.Domain.Entities;
using ECommerceOrderSystem.Domain.Enums;
using ECommerceOrderSystem.Domain.Events;
using ECommerceOrderSystem.Domain.ValueObjects;

namespace ECommerceOrderSystem.Application.Orders.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Result<CreateOrderResponse>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly ICacheService _cacheService;
        private readonly IValidator<CreateOrderCommand> _validator;
        private readonly ILogger<CreateOrderHandler> _logger;

        public CreateOrderHandler(
    IOrderRepository orderRepository,
    IEventPublisher eventPublisher,
    ICacheService cacheService,
    IValidator<CreateOrderCommand> validator,
    ILogger<CreateOrderHandler> logger)
{
    _orderRepository = orderRepository;
    _eventPublisher = eventPublisher;
    _cacheService = cacheService;
    _validator = validator;
    _logger = logger;
}

        public async Task<Result<CreateOrderResponse>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            try
            {
                // 1. Validation
                var validationResult = await _validator.ValidateAsync(request, cancellationToken);
                if (!validationResult.IsValid)
                {
                    var errors = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                    _logger.LogWarning("Order validation failed: {Errors}", errors);
                    return Result<CreateOrderResponse>.Failure($"Validation failed: {errors}");
                }

                // 2. Create Order Entity
                var paymentMethod = Enum.Parse<PaymentMethod>(request.PaymentMethod, true);
                var order = new Order(
                    new UserId(request.UserId),
                    new ProductId(request.ProductId),
                    request.Quantity,
                    paymentMethod);

                // 3. Save to Database
                var savedOrder = await _orderRepository.AddAsync(order, cancellationToken);
                _logger.LogInformation("Order created successfully. OrderId: {OrderId}", savedOrder.Id);

                // 4. Invalidate user's orders cache
                var cacheKey = $"user_orders_{request.UserId}";
                await _cacheService.RemoveAsync(cacheKey, cancellationToken);

                // 5. Publish Event
                var orderPlacedEvent = new OrderPlacedEvent(
                    savedOrder.Id,
                    request.UserId,
                    request.ProductId,
                    request.Quantity,
                    request.PaymentMethod);

                await _eventPublisher.PublishAsync(orderPlacedEvent, cancellationToken);
                _logger.LogInformation("OrderPlacedEvent published for OrderId: {OrderId}", savedOrder.Id);

                // 6. Return Response
                var response = new CreateOrderResponse
                {
                    OrderId = savedOrder.Id,
                    Status = savedOrder.Status.ToString(),
                    CreatedAt = savedOrder.CreatedAt
                };

                return Result<CreateOrderResponse>.Success(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating order for UserId: {UserId}", request.UserId);
                return Result<CreateOrderResponse>.Failure("An error occurred while creating the order");
            }
        }
    }
}