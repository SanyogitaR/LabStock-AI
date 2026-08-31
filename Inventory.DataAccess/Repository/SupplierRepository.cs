using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory.DataAccess.Repository
{
    /// <summary>
    /// Repository implementation for Supplier entity operations.
    /// </summary>
    public class SupplierRepository : Repository<Supplier>, ISupplierRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="SupplierRepository"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public SupplierRepository(AppDbContext db) : base(db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
        }

        /// <inheritdoc/>
        public void Update(Supplier obj)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj));

            _db.Suppliers.Update(obj);
        }

        /// <inheritdoc/>
        public async Task<Supplier?> GetByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be null or empty.", nameof(email));

            return await _db.Suppliers
                .FirstOrDefaultAsync(s => s.ContactEmail.ToLower() == email.ToLower());
        }
    }
}
