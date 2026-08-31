using System.Linq.Expressions;

namespace Inventory.DataAccess.Repository.IRepository
{
    /// <summary>
    /// Generic repository interface for common data access operations.
    /// </summary>
    /// <typeparam name="T">The entity type this repository manages.</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Retrieves all entities of type T, optionally including related entities.
        /// </summary>
        /// <param name="includeProperties">Comma-separated list of navigation properties to include.</param>
        /// <param name="tracked">Indicates whether entities should be tracked by the context. Default is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the collection of entities.</returns>
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null, bool tracked = true);

        /// <summary>
        /// Retrieves a single entity that matches the specified filter, optionally including related entities.
        /// </summary>
        /// <param name="filter">An expression to filter entities.</param>
        /// <param name="includeProperties">Comma-separated list of navigation properties to include.</param>
        /// <param name="tracked">Indicates whether the entity should be tracked by the context. Default is true.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
        Task<T?> GetAsync(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = true);

        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="entity">The entity to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddAsync(T entity);

        /// <summary>
        /// Adds a range of entities to the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to add.</param>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Removes an entity from the repository.
        /// </summary>
        /// <param name="entity">The entity to remove.</param>
        void Remove(T entity);

        /// <summary>
        /// Removes a range of entities from the repository.
        /// </summary>
        /// <param name="entities">The collection of entities to remove.</param>
        void RemoveRange(IEnumerable<T> entities);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="entity">The entity to update.</param>
        void Update(T entity);
    }
}
