using System.Linq.Expressions;
using SsoCore.Domain.Common;

namespace SsoCore.Application.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        Task<T?> AddAsync(T entity);
        Task<bool> UpdateAsync(T entity);
        Task<bool> DeleteAsync(T entity);
        Task<T?> GetByIdAsync(long id);
        Task<T?> GetSingleAsync(Func<IQueryable<T>, IQueryable<T>>? filter = null, params string[] includes);
        Task<T?> GetSingleAsync(Func<IQueryable<T>, IQueryable<T>>? filter = null, params Expression<Func<T, object>>[] includes);
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? filter = null, params string[] includes);
        Task<Pageable<T>> GetAllAsync(Func<IQueryable<T>, IQueryable<T>>? filter = null, int pageNumber = 1, int pageSize = 10);
        IQueryable<T> Query(Func<IQueryable<T>, IQueryable<T>>? filter = null);
    }
}
