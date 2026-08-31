using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository.IRepository
{
    /// <summary>
    /// Repository interface for PurchaseOrder entities.
    /// </summary>
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        /// <summary>
        /// Updates an existing purchase order in the database.
        /// </summary>
        /// <param name="obj">The purchase order to update.</param>
        void Update(PurchaseOrder obj);
    }
}
