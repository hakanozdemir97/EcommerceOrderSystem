using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ECommerceOrderSystem.Application.Common.Interfaces;
using ECommerceOrderSystem.Infrastructure.Persistence;

namespace ECommerceOrderSystem.Infrastructure.Services
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly("ECommerceOrderSystem.Infrastructure")));

            // Repositories
            services.AddScoped<IOrderRepository, ECommerceOrderSystem.Infrastructure.Persistence.Repositories.OrderRepository>();

            // Messaging
            services.AddSingleton<IEventPublisher, ECommerceOrderSystem.Infrastructure.Messaging.RabbitMQEventPublisher>();

            // Caching
            services.AddSingleton<ICacheService, ECommerceOrderSystem.Infrastructure.Caching.RedisCacheService>();

            return services;
        }
    }
}