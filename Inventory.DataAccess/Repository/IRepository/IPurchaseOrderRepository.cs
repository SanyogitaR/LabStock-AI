using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository.IRepository
{
    public interface IPurchaseOrderRepository : IRepository<PurchaseOrder>
    {
        void Update(PurchaseOrder obj);
    }
}