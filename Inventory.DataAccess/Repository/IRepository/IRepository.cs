using System.Linq.Expressions;

namespace Inventory.DataAccess.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> GetAll();

        T? GetFirstOrDefault(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null);

        Task<IEnumerable<T>> GetAllAsync(
            string? includeProperties = null);

        Task<T?> GetAsync(
            Expression<Func<T, bool>>? filter = null,
            string? includeProperties = null);

        void Add(T entity);

        Task AddAsync(T entity);

        void Remove(T entity);

        void RemoveRange(IEnumerable<T> entity);
    }
}