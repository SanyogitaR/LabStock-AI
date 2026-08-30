using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.DataAccess.Repository
{
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        private readonly AppDbContext _db;

        public SupplierRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Supplier obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            _db.Suppliers.Update(obj);
        }

        public async Task<Supplier?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(
                    "Email cannot be null or empty.",
                    nameof(email));

            return await _db.Suppliers
                .FirstOrDefaultAsync(s =>
                    s.ContactEmail.ToLower() == email.ToLower());
        }
    }
}