using System.Linq.Expressions;
using CachingDemo.Application.Interfaces;
using CachingDemo.Domain.Models;
using CachingDemo.Infrastructure.Data;
using CachingDemo.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Distributed;

namespace CachingDemo.Infrastructure.cache;

public class EmployeeCacheRepo : EmployeeRepository
{
    private IDistributedCache distributedCache {get; set;}
    public EmployeeCacheRepo(ApplicationDbContext context, IDistributedCache _distributedCache) : base(context)
    {
        distributedCache = _distributedCache;
    }

    public new async Task<List<TResult>> GetList<TResult>(Expression<Func<Employee, TResult>> selector, Expression<Func<Employee, bool>>? predicate = null)
    {
        string cacheKey = $"employees";
        List<Employee> cachedEmployees = null;
        var cachedData = await distributedCache.GetStringAsync(cacheKey);
        if (cachedData != null)
        {
            cachedEmployees = System.Text.Json.JsonSerializer.Deserialize<List<Employee>>(cachedData);
        }
        else
        {
            cachedEmployees = await base.GetList(e => e, null); 
            var serializedData = System.Text.Json.JsonSerializer.Serialize(cachedEmployees);
            await distributedCache.SetStringAsync(cacheKey, serializedData, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(20)
            });
        }

        IEnumerable<Employee> filtered = cachedEmployees;

        if (predicate != null)
        {
            filtered = filtered.AsQueryable().Where(predicate);
        }

        return filtered.Select(selector.Compile()).ToList();
    }
}
