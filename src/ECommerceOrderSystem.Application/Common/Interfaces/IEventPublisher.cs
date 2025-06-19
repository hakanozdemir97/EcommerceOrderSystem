using System.Threading;
using System.Threading.Tasks;

namespace ECommerceOrderSystem.Application.Common.Interfaces
{
    public interface IEventPublisher
    {
        Task PublishAsync<T>(T @event, CancellationToken cancellationToken = default) where T : class;
    }
}