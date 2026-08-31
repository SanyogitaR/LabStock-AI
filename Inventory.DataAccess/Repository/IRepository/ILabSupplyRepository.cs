using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository.IRepository
{
    /// <summary>
    /// Repository interface for LabSupply entity with specific business operations.
    /// </summary>
    public interface ILabSupplyRepository : IRepository<LabSupply>
    {
        /// <summary>
        /// Updates an existing lab supply entity.
        /// </summary>
        /// <param name="obj">The lab supply entity to update.</param>
        void Update(LabSupply obj);

        /// <summary>
        /// Retrieves all lab supplies that need reordering (quantity at or below reorder point).
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains the collection of supplies needing reorder.</returns>
        Task<IEnumerable<LabSupply>> GetSuppliesNeedingReorderAsync();
    }
}
