using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using ECommerceOrderSystem.Application.Common.Interfaces;
using ECommerceOrderSystem.Domain.Entities;
using ECommerceOrderSystem.Domain.ValueObjects;

namespace ECommerceOrderSystem.Infrastructure.Persistence.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderRepository> _logger;

        public OrderRepository(ApplicationDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Order saved to database. OrderId: {OrderId}", order.Id);
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving order to database. OrderId: {OrderId}", order.Id);
                throw;
            }
        }

        public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                var order = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
                
                if (order != null)
                {
                    _logger.LogInformation("Order retrieved from database. OrderId: {OrderId}", id);
                }
                
                return order;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving order from database. OrderId: {OrderId}", id);
                throw;
            }
        }

        public async Task<IEnumerable<Order>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default)
        {
            try
            {
                var orders = await _context.Orders
                    .Where(o => o.UserId == userId)
                    .OrderByDescending(o => o.CreatedAt)
                    .ToListAsync(cancellationToken);

                _logger.LogInformation("Retrieved {Count} orders for UserId: {UserId}", orders.Count, userId.Value);
                return orders;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving orders for UserId: {UserId}", userId.Value);
                throw;
            }
        }

        public async Task UpdateAsync(Order order, CancellationToken cancellationToken = default)
        {
            try
            {
                _context.Orders.Update(order);
                await _context.SaveChangesAsync(cancellationToken);
                _logger.LogInformation("Order updated in database. OrderId: {OrderId}", order.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating order in database. OrderId: {OrderId}", order.Id);
                throw;
            }
        }
    }
}