using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.DataAccess.Repository
{
    /// <summary>
    /// Repository implementation for LabSupply entity operations.
    /// </summary>
    public class LabSupplyRepository : Repository<LabSupply>, ILabSupplyRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="LabSupplyRepository"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public LabSupplyRepository(AppDbContext db) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <inheritdoc/>
        public void Update(LabSupply obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            var objFromDb = _db.LabSupplies.FirstOrDefault(u => u.SupplyID == obj.SupplyID);
            
            if (objFromDb != null)
            {
                objFromDb.SupplyName = obj.SupplyName;
                objFromDb.QuantityOnHand = obj.QuantityOnHand;
                objFromDb.ReorderPoint = obj.ReorderPoint;
                objFromDb.SupplierID = obj.SupplierID;
                
                if (!string.IsNullOrEmpty(obj.ImageURL))
                {
                    objFromDb.ImageURL = obj.ImageURL;
                }
            }
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<LabSupply>> GetSuppliesNeedingReorderAsync()
        {
            return await _db.LabSupplies
                .Include(l => l.Supplier)
                .Where(l => l.QuantityOnHand <= l.ReorderPoint)
                .OrderBy(l => l.SupplyName)
                .ToListAsync();
        }
    }
}
