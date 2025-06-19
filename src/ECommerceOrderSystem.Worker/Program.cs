using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;
using ECommerceOrderSystem.Worker;
using ECommerceOrderSystem.Application.Common.Interfaces;

// Serilog configuration
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/worker-.txt", rollingInterval: RollingInterval.Day)
    .CreateLogger();

var builder = Host.CreateApplicationBuilder(args);

// Add Serilog
builder.Services.AddSerilog();

// Add the worker service
builder.Services.AddHostedService<Worker>();

// Add mock cache service for demo
builder.Services.AddSingleton<ICacheService, MockCacheService>();

var host = builder.Build();

try
{
    Log.Information("Starting Worker Service");
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker Service terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

// Mock Cache Service
public class MockCacheService : ICacheService
{
    private readonly Dictionary<string, object> _cache = new();
    private readonly ILogger<MockCacheService> _logger;

    public MockCacheService(ILogger<MockCacheService> logger)
    {
        _logger = logger;
    }

    public Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
    {
        _cache.TryGetValue(key, out var value);
        _logger.LogDebug("Cache GET: {Key} = {Value}", key, value ?? "NULL");
        return Task.FromResult(value as T);
    }

    public Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
    {
        _cache[key] = value!;
        _logger.LogInformation("Cache SET: {Key} = {Value}", key, value);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        _cache.Remove(key);
        _logger.LogDebug("Cache REMOVE: {Key}", key);
        return Task.CompletedTask;
    }

    public Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Cache REMOVE PATTERN: {Pattern}", pattern);
        return Task.CompletedTask;
    }
}