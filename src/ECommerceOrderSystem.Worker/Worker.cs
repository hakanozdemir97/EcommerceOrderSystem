using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ECommerceOrderSystem.Domain.Events;
using ECommerceOrderSystem.Application.Common.Interfaces;

namespace ECommerceOrderSystem.Worker;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IServiceProvider _serviceProvider;
    private IConnection? _connection;
    private IModel? _channel;

    public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await SetupRabbitMQ();
        
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            await Task.Delay(10000, stoppingToken); // Keep alive
        }
    }

    private async Task SetupRabbitMQ()
    {
        try
        {
            // For demo purposes, use mock processing
            _logger.LogInformation("Worker started - Using mock message processing");
            
            // Simulate order processing
            _ = Task.Run(async () =>
            {
                while (true)
                {
                    await Task.Delay(5000); // Process every 5 seconds
                    await ProcessMockOrder();
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to setup RabbitMQ connection");
        }
    }

    private async Task ProcessMockOrder()
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var cacheService = scope.ServiceProvider.GetRequiredService<ICacheService>();
            
            var orderId = Guid.NewGuid();
            var timestamp = DateTimeOffset.UtcNow;
            
            _logger.LogInformation("Processing mock order: {OrderId}", orderId);
            
            // Simulate processing delay
            await Task.Delay(2000);
            
            // Store processing log in Redis
            var logMessage = $"Processed at {timestamp:yyyy-MM-dd HH:mm:ss}";
            await cacheService.SetAsync($"order_log_{orderId}", logMessage, TimeSpan.FromHours(1));
            
            _logger.LogInformation("Order processed successfully: {OrderId}", orderId);
            
            // Simulate notification
            _logger.LogInformation("Notification sent for order: {OrderId}", orderId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing mock order");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Worker stopping...");
        
        _channel?.Close();
        _connection?.Close();
        
        await base.StopAsync(cancellationToken);
    }
}