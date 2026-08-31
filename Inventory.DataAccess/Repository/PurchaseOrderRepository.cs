using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository
{
    /// <summary>
    /// Repository implementation for PurchaseOrder entities.
    /// </summary>
    public class PurchaseOrderRepository : Repository<PurchaseOrder>, IPurchaseOrderRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="PurchaseOrderRepository"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public PurchaseOrderRepository(AppDbContext db) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <summary>
        /// Updates an existing purchase order in the database.
        /// </summary>
        /// <param name="obj">The purchase order to update.</param>
        public void Update(PurchaseOrder obj)
        {
            _db.PurchaseOrders.Update(obj);
        }
    }
}
