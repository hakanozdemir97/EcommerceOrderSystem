using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using ECommerceOrderSystem.Application.Common.Interfaces;
using ECommerceOrderSystem.Application.Common.Models;
using ECommerceOrderSystem.Domain.ValueObjects;

namespace ECommerceOrderSystem.Application.Orders.Queries.GetUserOrders
{
    public class GetUserOrdersHandler : IRequestHandler<GetUserOrdersQuery, Result<IEnumerable<UserOrderResponse>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly ICacheService _cacheService;
        private readonly ILogger<GetUserOrdersHandler> _logger;
        private static readonly TimeSpan CacheTtl = TimeSpan.FromMinutes(2);

        public GetUserOrdersHandler(
            IOrderRepository orderRepository,
            ICacheService cacheService,
            ILogger<GetUserOrdersHandler> logger)
        {
            _orderRepository = orderRepository;
            _cacheService = cacheService;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<UserOrderResponse>>> Handle(GetUserOrdersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(request.UserId))
                {
                    return Result<IEnumerable<UserOrderResponse>>.Failure("User ID is required");
                }

                var cacheKey = $"user_orders_{request.UserId}";

                // Try to get from cache first
                var cachedOrders = await _cacheService.GetAsync<IEnumerable<UserOrderResponse>>(cacheKey, cancellationToken);
                if (cachedOrders != null)
                {
                    _logger.LogInformation("Orders retrieved from cache for UserId: {UserId}", request.UserId);
                    return Result<IEnumerable<UserOrderResponse>>.Success(cachedOrders);
                }

                // Get from database
                var orders = await _orderRepository.GetByUserIdAsync(new UserId(request.UserId), cancellationToken);
                var orderResponses = orders.Select(o => new UserOrderResponse
                {
                    OrderId = o.Id,
                    ProductId = o.ProductId.Value,
                    Quantity = o.Quantity,
                    PaymentMethod = o.PaymentMethod.ToString(),
                    Status = o.Status.ToString(),
                    CreatedAt = o.CreatedAt,
                    ProcessedAt = o.ProcessedAt
                }).ToList();

                // Cache the result
                await _cacheService.SetAsync(cacheKey, orderResponses, CacheTtl, cancellationToken);
                
                _logger.LogInformation("Orders retrieved from database and cached for UserId: {UserId}. Count: {Count}", 
                    request.UserId, orderResponses.Count);

                return Result<IEnumerable<UserOrderResponse>>.Success(orderResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for UserId: {UserId}", request.UserId);
                return Result<IEnumerable<UserOrderResponse>>.Failure("An error occurred while retrieving orders");
            }
        }
    }
}