using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository
{
    public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        private readonly AppDbContext _db;

        public PurchaseOrderRepository(AppDbContext db) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        public void Update(PurchaseOrder obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            _db.Set<PurchaseOrder>().Update(obj);
        }
    }
}
