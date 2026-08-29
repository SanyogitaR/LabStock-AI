using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository.IRepository
{
    /// <summary>
    /// Repository interface for Supplier entity with specific business operations.
    /// </summary>
    public interface ISupplierRepository : IRepository<Supplier>
    {
        /// <summary>
        /// Updates an existing supplier entity.
        /// </summary>
        /// <param name="obj">The supplier entity to update.</param>
        void Update(Supplier obj);

        /// <summary>
        /// Retrieves a supplier by email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the supplier if found; otherwise, null.</returns>
        Task<Supplier?> GetByEmailAsync(string email);
    }
}
