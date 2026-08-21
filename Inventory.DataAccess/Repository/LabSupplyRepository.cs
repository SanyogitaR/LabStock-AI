using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository
{
    /// <summary>
    /// Repository implementation for LabSupply-specific data operations.
    /// </summary>
    public class LabSupplyRepository : Repository<LabSupply>, ILabSupplyRepository
    {
        private readonly ApplicationDbContext _db;

        public LabSupplyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(LabSupply labSupply)
        {
            _db.LabSupplies.Update(labSupply);
        }
    }
}
