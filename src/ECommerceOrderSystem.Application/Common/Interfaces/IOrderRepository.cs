using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ECommerceOrderSystem.Domain.Entities;
using ECommerceOrderSystem.Domain.ValueObjects;

namespace ECommerceOrderSystem.Application.Common.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> AddAsync(Order order, CancellationToken cancellationToken = default);
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<Order>> GetByUserIdAsync(UserId userId, CancellationToken cancellationToken = default);
        Task UpdateAsync(Order order, CancellationToken cancellationToken = default);
    }
}