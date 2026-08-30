using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository
{
    public class LabSupplyRepository : Repository<LabSupply>, ILabSupplyRepository
    {
        private readonly AppDbContext _db;

        public LabSupplyRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(LabSupply labSupply)
        {
            _db.LabSupplies.Update(labSupply);
        }
    }
}