using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using ECommerceOrderSystem.Application.Common.Interfaces;

namespace ECommerceOrderSystem.Infrastructure.Caching
{
    public class RedisCacheService : ICacheService, IDisposable
    {
        private readonly IDatabase _database;
        private readonly IConnectionMultiplexer _connection;
        private readonly ILogger<RedisCacheService> _logger;

        public RedisCacheService(IConfiguration configuration, ILogger<RedisCacheService> logger)
        {
            _logger = logger;

            try
            {
                var connectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
                
                var configurationOptions = ConfigurationOptions.Parse(connectionString);
                configurationOptions.AbortOnConnectFail = false;

                _connection = ConnectionMultiplexer.Connect(configurationOptions);
                _database = _connection.GetDatabase();

                _logger.LogInformation("Redis connection established successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to establish Redis connection");
                throw;
            }
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var value = await _database.StringGetAsync(key);
                
                if (!value.HasValue)
                {
                    _logger.LogDebug("Cache miss for key: {Key}", key);
                    return null;
                }

                var result = JsonSerializer.Deserialize<T>(value!, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                _logger.LogDebug("Cache hit for key: {Key}", key);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting value from cache. Key: {Key}", key);
                return null; // Return null instead of throwing to prevent cache failures from breaking the app
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null, CancellationToken cancellationToken = default) where T : class
        {
            try
            {
                var serializedValue = JsonSerializer.Serialize(value, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await _database.StringSetAsync(key, serializedValue, expiry);
                
                _logger.LogDebug("Value cached with key: {Key}, Expiry: {Expiry}", key, expiry?.ToString() ?? "No expiry");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting value in cache. Key: {Key}", key);
                // Don't throw - cache failures shouldn't break the application
            }
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            try
            {
                await _database.KeyDeleteAsync(key);
                _logger.LogDebug("Cache key removed: {Key}", key);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing key from cache. Key: {Key}", key);
                // Don't throw - cache failures shouldn't break the application
            }
        }

        public async Task RemoveByPatternAsync(string pattern, CancellationToken cancellationToken = default)
        {
            try
            {
                var server = _connection.GetServer(_connection.GetEndPoints().First());
                var keys = server.Keys(pattern: pattern);

                foreach (var key in keys)
                {
                    await _database.KeyDeleteAsync(key);
                }

                _logger.LogDebug("Cache keys removed by pattern: {Pattern}", pattern);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing keys by pattern from cache. Pattern: {Pattern}", pattern);
                // Don't throw - cache failures shouldn't break the application
            }
        }

        public void Dispose()
        {
            try
            {
                _connection?.Dispose();
                _logger.LogInformation("Redis connection disposed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error disposing Redis connection");
            }
        }
    }
}