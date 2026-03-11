# Caching Guide - Complete Reference

## Table of Contents
1. [When to Use Caching](#when-to-use-caching)
2. [Types of Caching](#types-of-caching)
3. [Implementation Steps](#implementation-steps)
4. [Core Caching Methods](#core-caching-methods)
5. [Code Examples](#code-examples)
6. [Best Practices](#best-practices)

---

## When to Use Caching

### ✅ Use Caching When:

1. **High Read-to-Write Ratio**
   - Data is read frequently but changed infrequently
   - Example: Department list, Product catalog, Configuration data

2. **Expensive Operations**
   - Database queries that involve joins or complex calculations
   - API calls to external services
   - Heavy computational operations

3. **Data Consistency is Not Critical**
   - Slight data staleness is acceptable
   - Example: User dashboard statistics, Cache expiry of 5-30 minutes

4. **Reduce Database Load**
   - When database is under heavy stress
   - To minimize latency for read operations

5. **Improve Performance**
   - Response time is critical
   - High traffic application

### ❌ Don't Use Caching When:

- **Real-time Data Required**: Stock prices, live sports scores
- **Highly Volatile Data**: Frequently changing data (every few seconds)
- **Security Sensitive**: Passwords, PII, confidential information
- **Small Dataset**: Caching overhead > benefit
- **Single User System**: No concurrent access benefit

---

## Types of Caching

### 1. IN-MEMORY CACHE (Microsoft.Extensions.Caching.Memory)

#### ✅ **Pros:**
- Very fast (in-process, no network latency)
- Low overhead
- Good for single-server applications
- No serialization required

#### ❌ **Cons:**
- Lost when application restarts
- Not shared across multiple server instances
- Limited by server RAM
- Not suitable for distributed systems

#### **Use When:**
- Single server application
- Data loss acceptable on restart
- Small to medium datasets
- Session-specific or user-specific data
- Application startup is frequent

#### **Example from Project:**
```csharp
// Department Cache using In-Memory Cache
public class DepartmentCashRepo : DepartmentRepository
{
    private readonly IMemoryCache _memoryCache;

    public async Task<List<TResult>> GetList<TResult>(
        Expression<Func<Department, TResult>> selector, 
        Expression<Func<Department, bool>>? predicate = null)
    {
        string cacheKey = $"departments";
        
        // Try to get from cache
        if (!_memoryCache.TryGetValue(cacheKey, out List<Department> cachedDepartments))
        {
            // Cache miss: fetch from database
            cachedDepartments = await base.GetList(d => d, null);
            
            // Store in cache for 20 minutes
            _memoryCache.Set(cacheKey, cachedDepartments, TimeSpan.FromMinutes(20));
        }

        // Apply filtering on cached data
        IEnumerable<Department> filtered = cachedDepartments;
        if (predicate != null)
        {
            filtered = filtered.AsQueryable().Where(predicate);
        }

        return filtered.Select(selector.Compile()).ToList();
    }
}
```

---

### 2. DISTRIBUTED CACHE (Redis / Microsoft.Extensions.Caching.Distributed)

#### ✅ **Pros:**
- Shared across multiple server instances
- Persists data (configurable)
- Survives application restart
- Supports clustering and replication
- Suitable for microservices architecture
- Can be accessed from different applications

#### ❌ **Cons:**
- Network latency
- Requires serialization/deserialization
- Additional infrastructure to maintain
- More complex setup
- Requires data serialization

#### **Use When:**
- Distributed/multi-server environment
- Load balancing across multiple instances
- Data persistence needed
- Multiple applications need shared cache
- Cloud-based or microservices architecture
- Real-time data sync across instances

#### **Example from Project:**
```csharp
// Employee Cache using Distributed Cache (Redis)
public class EmployeeCacheRepo : EmployeeRepository
{
    private IDistributedCache distributedCache { get; set; }

    public async Task<List<TResult>> GetList<TResult>(
        Expression<Func<Employee, TResult>> selector,
        Expression<Func<Employee, bool>>? predicate = null)
    {
        string cacheKey = $"employees";
        List<Employee> cachedEmployees = null;

        // Try to get from distributed cache
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        
        if (cachedData != null)
        {
            // Cache hit: deserialize from JSON
            cachedEmployees = System.Text.Json.JsonSerializer
                .Deserialize<List<Employee>>(cachedData);
        }
        else
        {
            // Cache miss: fetch from database
            cachedEmployees = await base.GetList(e => e, null);
            
            // Serialize and store in Redis with 20 minutes expiry
            var serializedData = System.Text.Json.JsonSerializer
                .Serialize(cachedEmployees);
            
            await distributedCache.SetStringAsync(
                cacheKey, 
                serializedData,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
                }
            );
        }

        // Apply filtering
        IEnumerable<Employee> filtered = cachedEmployees;
        if (predicate != null)
        {
            filtered = filtered.AsQueryable().Where(predicate);
        }

        return filtered.Select(selector.Compile()).ToList();
    }
}
```

---

### Comparison Table

| Feature | In-Memory | Distributed (Redis) |
|---------|-----------|-------------------|
| **Speed** | ⚡⚡⚡ Very Fast | ⚡⚡ Fast |
| **Server Instance** | Single | Multiple |
| **Persistence** | ❌ No | ✅ Yes |
| **Network Latency** | None | Yes |
| **RAM Usage** | Server | Dedicated |
| **Serialization** | Not needed | Required |
| **Scalability** | ❌ Limited | ✅ Excellent |
| **Setup Complexity** | ✅ Easy | Medium |

---

## Core Caching Methods

### IN-MEMORY CACHE METHODS

#### **1. TryGetValue() - Retrieve with Check**
```csharp
if (_memoryCache.TryGetValue("key", out var value))
{
    // Cache hit
    return value;
}
// Cache miss - fetch from source
```

#### **2. Set() - Store Value**
```csharp
_memoryCache.Set(
    "key",                              // Cache key
    data,                               // Value to cache
    TimeSpan.FromMinutes(20)            // Expiration time
);
```

#### **3. GetOrCreate() - Get with Lazy Creation**
```csharp
var value = await _memoryCache.GetOrCreateAsync(
    "key",
    async entry =>
    {
        entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20);
        return await database.GetDataAsync();
    }
);
```

#### **4. Remove() - Delete Single Entry**
```csharp
_memoryCache.Remove("key");
```

---

### DISTRIBUTED CACHE METHODS (Redis)

#### **1. GetStringAsync() - Retrieve String Value**
```csharp
var cachedData = await _distributedCache.GetStringAsync("key");
if (cachedData != null)
{
    var obj = JsonSerializer.Deserialize<MyClass>(cachedData);
}
```

#### **2. SetStringAsync() - Store String Value**
```csharp
var serialized = JsonSerializer.Serialize(data);
await _distributedCache.SetStringAsync(
    "key",
    serialized,
    new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20),
        // Optional: SlidingExpiration = TimeSpan.FromMinutes(5)
    }
);
```

#### **3. GetAsync() - Retrieve Byte Array**
```csharp
var bytes = await _distributedCache.GetAsync("key");
if (bytes != null)
{
    var json = Encoding.UTF8.GetString(bytes);
    var obj = JsonSerializer.Deserialize<MyClass>(json);
}
```

#### **4. SetAsync() - Store Byte Array**
```csharp
var bytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(data));
await _distributedCache.SetAsync(
    "key",
    bytes,
    new DistributedCacheEntryOptions()
);
```

#### **5. RemoveAsync() - Delete Single Entry**
```csharp
await _distributedCache.RemoveAsync("key");
```

#### **6. RefreshAsync() - Reset Expiration Timer**
```csharp
await _distributedCache.RefreshAsync("key");
```

---

## Code Examples

### BASIC: Simple In-Memory Cache

```csharp
// 🔵 BASIC LEVEL
public class ProductRepository
{
    private readonly IMemoryCache _cache;
    private readonly IProductDatabase _db;

    public async Task<List<Product>> GetAllProducts()
    {
        const string cacheKey = "all_products";
        
        // Check cache first
        if (_cache.TryGetValue(cacheKey, out List<Product> products))
        {
            return products; // Cache hit!
        }

        // Cache miss - get from database
        products = await _db.GetAllProductsAsync();
        
        // Store in cache for 30 minutes
        _cache.Set(cacheKey, products, TimeSpan.FromMinutes(30));
        
        return products;
    }
}
```

---

### MEDIUM: Key-Based Distributed Cache with Expiration

```csharp
// 🟡 MEDIUM LEVEL
public class UserRepository
{
    private readonly IDistributedCache _cache;
    private readonly IUserDatabase _db;
    private const int CACHE_EXPIRY_MINUTES = 20;

    public async Task<User> GetUserByIdAsync(int userId)
    {
        string cacheKey = $"user_{userId}";
        
        // Try to get from Redis
        var cachedData = await _cache.GetStringAsync(cacheKey);
        
        if (cachedData != null)
        {
            return JsonSerializer.Deserialize<User>(cachedData);
        }

        // Cache miss - fetch from database
        var user = await _db.GetUserByIdAsync(userId);
        
        if (user != null)
        {
            // Serialize and cache
            var serialized = JsonSerializer.Serialize(user);
            await _cache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = 
                        TimeSpan.FromMinutes(CACHE_EXPIRY_MINUTES)
                }
            );
        }

        return user;
    }

    public async Task InvalidateUserCache(int userId)
    {
        string cacheKey = $"user_{userId}";
        await _cache.RemoveAsync(cacheKey);
    }

    public async Task UpdateUserAsync(User user)
    {
        // Update in database
        await _db.UpdateUserAsync(user);
        
        // Remove from cache to force refresh on next read
        await InvalidateUserCache(user.Id);
    }
}
```

---

### ADVANCED: Multi-Level Cache with Cache Decorator Pattern

```csharp
// 🔴 ADVANCED LEVEL
public class CacheDecoratedUserRepository : IUserRepository
{
    private readonly IUserRepository _innerRepository;
    private readonly IMemoryCache _l1Cache;        // Fast local cache
    private readonly IDistributedCache _l2Cache;   // Shared Redis cache
    
    private const int L1_EXPIRY_MINUTES = 5;
    private const int L2_EXPIRY_MINUTES = 20;
    private const string CACHE_KEY_PREFIX = "user_";

    public async Task<User> GetUserByIdAsync(int userId)
    {
        string cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
        
        // L1: Check In-Memory Cache
        if (_l1Cache.TryGetValue(cacheKey, out User cachedUser))
        {
            return cachedUser;
        }

        // L2: Check Distributed Cache (Redis)
        var l2Data = await _l2Cache.GetStringAsync(cacheKey);
        if (l2Data != null)
        {
            var user = JsonSerializer.Deserialize<User>(l2Data);
            // Also cache in L1 for quick access
            _l1Cache.Set(cacheKey, user, TimeSpan.FromMinutes(L1_EXPIRY_MINUTES));
            return user;
        }

        // L3: Database
        var dbUser = await _innerRepository.GetUserByIdAsync(userId);
        
        if (dbUser != null)
        {
            var serialized = JsonSerializer.Serialize(dbUser);
            
            // Store in L2 cache
            await _l2Cache.SetStringAsync(
                cacheKey,
                serialized,
                new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = 
                        TimeSpan.FromMinutes(L2_EXPIRY_MINUTES)
                }
            );
            
            // Store in L1 cache
            _l1Cache.Set(cacheKey, dbUser, 
                TimeSpan.FromMinutes(L1_EXPIRY_MINUTES));
        }

        return dbUser;
    }

    public async Task InvalidateAsync(int userId)
    {
        string cacheKey = $"{CACHE_KEY_PREFIX}{userId}";
        
        // Invalidate both cache levels
        _l1Cache.Remove(cacheKey);
        await _l2Cache.RemoveAsync(cacheKey);
    }

    public async Task InvalidateByPatternAsync(string pattern)
    {
        // Remove all matching keys from Redis
        // Note: Redis doesn't support pattern-based removal directly
        // Need custom implementation or use Redis SCAN command
        
        // For now, invalidate known keys
        var keys = new[] { /* list of keys */ };
        foreach (var key in keys)
        {
            if (key.Contains(pattern))
            {
                await _l2Cache.RemoveAsync(key);
            }
        }
    }
}
```

---

### ADVANCED: Caching Strategy Patterns

```csharp
// 🔴 ADVANCED: Cache Invalidation Strategies

// 1. Time-Based Expiration (TTL)
await _cache.SetStringAsync(
    key,
    value,
    new DistributedCacheEntryOptions
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
    }
);

// 2. Sliding Expiration (Resets on Access)
await _cache.SetStringAsync(
    key,
    value,
    new DistributedCacheEntryOptions
    {
        SlidingExpiration = TimeSpan.FromMinutes(5) // Resets on each access
    }
);

// 3. Event-Based Invalidation (After Update)
public async Task UpdateUserAsync(User user)
{
    await _database.UpdateAsync(user);
    await _cache.RemoveAsync($"user_{user.Id}"); // Immediate invalidation
}

// 4. Manual Refresh
public async Task RefreshCacheAsync(string key)
{
    await _cache.RemoveAsync(key); // Clear cache
    // Next request will fetch fresh data
}

// 5. Cache Warming (Pre-load)
public async Task WarmCacheAsync()
{
    var criticalData = await _database.GetCriticalDataAsync();
    await _cache.SetStringAsync("critical_data", 
        JsonSerializer.Serialize(criticalData));
}
```

---

## Best Practices

### ✅ DO's

1. **Use Meaningful Cache Keys**
   ```csharp
   // ✅ Good
   string key = $"user_{userId}_profile";
   string key = $"products_category_{categoryId}_page_{pageNumber}";
   ```

2. **Set Appropriate Expiration**
   ```csharp
   // ✅ Good - Consider data freshness requirements
   var options = new DistributedCacheEntryOptions
   {
       AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
   };
   ```

3. **Handle Cache Misses Gracefully**
   ```csharp
   // ✅ Good - Fallback to database on cache miss
   var data = await _cache.GetStringAsync(key);
   if (data == null)
   {
       data = await _database.FetchAsync();
   }
   ```

4. **Invalidate Cache on Updates**
   ```csharp
   // ✅ Good - Keep cache consistent
   await _database.UpdateAsync(item);
   await _cache.RemoveAsync(cacheKey);
   ```

5. **Serialize Large Objects**
   ```csharp
   // ✅ Good - Use Json for distributed cache
   var json = JsonSerializer.Serialize(largeObject);
   await _cache.SetStringAsync(key, json);
   ```

---

### ❌ DON'Ts

1. **Don't Cache Without Expiration**
   ```csharp
   // ❌ Bad - Data becomes stale
   _cache.Set(key, data); // No expiration!
   
   // ✅ Good
   _cache.Set(key, data, TimeSpan.FromMinutes(20));
   ```

2. **Don't Cache Sensitive Data**
   ```csharp
   // ❌ Bad - Security risk
   _cache.Set("user_password", password);
   
   // ✅ Good - Never cache passwords, tokens, PII
   ```

3. **Don't Cache Without Handling null**
   ```csharp
   // ❌ Bad - Doesn't handle non-existent data
   if (_cache.TryGetValue(key, out var data))
   {
       return data;
   }
   
   // ✅ Good - Store null indicators
   var data = _cache.Get(key) ?? await FetchAsync();
   ```

4. **Don't Ignore Network Calls**
   ```csharp
   // ❌ Bad - No error handling
   var json = await _cache.GetStringAsync(key);
   return JsonSerializer.Deserialize<T>(json);
   
   // ✅ Good - Handle Redis failures
   try
   {
       var json = await _cache.GetStringAsync(key);
       if (json != null) return JsonSerializer.Deserialize<T>(json);
   }
   catch (Exception ex)
   {
       _logger.LogWarning($"Cache error: {ex.Message}");
   }
   return await _database.FetchAsync();
   ```

5. **Don't Store Large Objects Without Need**
   ```csharp
   // ❌ Bad - Memory waste
   _cache.Set("large_list", hugeList);
   
   // ✅ Good - Cache only what's needed
   _cache.Set("user_summary", new UserSummary { Id, Name });
   ```

---

## Quick Reference

### In-Memory Cache Checklist
- ✅ Single-server application
- ✅ Data loss on restart acceptable
- ✅ High-frequency reads
- ✅ Small to medium datasets
- ✅ No distributed system

### Distributed Cache Checklist
- ✅ Multi-server/load-balanced setup
- ✅ Data persistence required
- ✅ Shared cache across services
- ✅ Microservices architecture
- ✅ Real-time sync across instances

### Caching Expiration Times
- **API results**: 5-15 minutes
- **Database queries**: 15-30 minutes
- **Lookup lists**: 30-60 minutes
- **Reference data**: 60+ minutes
- **Session data**: 30 minutes

---

## Project Implementation Summary

This project demonstrates both caching types:

**DepartmentCashRepo** (In-Memory):
- Faster response
- Single instance
- Data lost on restart
- Used for reference data

**EmployeeCacheRepo** (Distributed/Redis):
- Shared across instances
- Persistent storage
- Network overhead
- Suitable for scalable apps

Both use **20-minute expiration** and follow the **Cache-Aside Pattern** (check cache → on miss, fetch from DB → store in cache).

---

## Interview Tips

✨ **Remember to mention:**
1. Cache-Aside Pattern (Lazy Loading)
2. Cache invalidation strategies
3. TTL (Time To Live) importance
4. Two-level caching benefits
5. When NOT to use caching
6. Cache stampede problem (solve with locks)
7. Memory vs Distributed trade-offs

✨ **Code Examples:**
- Show the repository pattern with injection
- Explain TryGetValue vs GetStringAsync
- Discuss serialization for Redis
- Mention expiration policies

---

**Last Updated**: March 11, 2026
**Project**: CachingDemo
