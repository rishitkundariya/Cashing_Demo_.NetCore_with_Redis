using System.Linq.Expressions;
using CachingDemo.Application.Interfaces;
using CachingDemo.Domain.Models;
using CachingDemo.Infrastructure.Data;
using CachingDemo.Infrastructure.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace CachingDemo.Infrastructure.cache;

public class DepartmentCashRepo : DepartmentRepository
{
    private readonly IMemoryCache _memoryCache;

    public DepartmentCashRepo(DepartmentRepository repository, ApplicationDbContext context, IMemoryCache memoryCache) : base(context)
    {
        _memoryCache = memoryCache;
    }

    public new async Task<List<TResult>> GetList<TResult>(Expression<Func<Department, TResult>> selector, Expression<Func<Department, bool>>? predicate = null)
    {
        string cacheKey = $"departments";
        if (!_memoryCache.TryGetValue(cacheKey, out List<Department> cachedDepartments))
        {
            cachedDepartments = await GetList(d => d, null); 
            _memoryCache.Set(cacheKey, cachedDepartments, TimeSpan.FromMinutes(20));
        }

        IEnumerable<Department> filtered = cachedDepartments;

        if (predicate != null)
        {
            filtered = filtered.AsQueryable().Where(predicate);
        }

        return filtered.Select(selector.Compile()).ToList();
    }
}
