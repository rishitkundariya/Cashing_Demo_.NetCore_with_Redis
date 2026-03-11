using System.Linq.Expressions;

namespace CachingDemo.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
    Task<bool> SaveChangesAsync();
    Task<List<TResult>> GetList<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? predicate = null);
    Task<TResult> GetFirstOrDefault<TResult>(Expression<Func<T, TResult>> selector, Expression<Func<T, bool>>? predicate = null);
}
