namespace CachingDemo.Application.Services;

/// <summary>
/// Redis Cache Service Interface
/// TODO: Implement this service using StackExchange.Redis
/// 
/// Required Methods:
/// - GetAsync<T>(string key) - Retrieve cached value by key
/// - SetAsync<T>(string key, T value, TimeSpan? expiration) - Store value with optional TTL
/// - RemoveAsync(string key) - Delete specific cache entry
/// - RemoveByPatternAsync(string pattern) - Delete multiple entries by pattern
/// 
/// Hints:
/// - Use IConnectionMultiplexer from StackExchange.Redis
/// - Use JsonSerializer for object serialization
/// - Handle JSON conversion for GetAsync and SetAsync
/// - Consider cache key patterns for invalidation strategy
/// </summary>
public interface ICacheService
{
    Task<T?> GetAsync<T>(string key);
    Task SetAsync<T>(string key, T value, TimeSpan? expiration = null);
    Task RemoveAsync(string key);
    Task RemoveByPatternAsync(string pattern);
}

