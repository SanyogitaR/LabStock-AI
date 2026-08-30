using Inventory.DataAccess.Data;
using Inventory.DataAccess.Repository.IRepository;

namespace Inventory.DataAccess.Repository
{
    /// <summary>
    /// Unit of Work implementation for managing repository instances and database transactions.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _db;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="UnitOfWork"/> class.
        /// </summary>
        /// <param name="db">The database context.</param>
        public UnitOfWork(AppDbContext db)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            Supplier = new SupplierRepository(_db);
            LabSupply = new LabSupplyRepository(_db);
            PurchaseOrder = new PurchaseOrderRepository(_db);
        }

        /// <inheritdoc/>
        public ISupplierRepository Supplier { get; private set; }

        /// <inheritdoc/>
        public ILabSupplyRepository LabSupply { get; private set; }

        /// <inheritdoc/>
        public IPurchaseOrderRepository PurchaseOrder { get; private set; }

        /// <inheritdoc/>
        public async Task<int> SaveAsync()
        {
            return await _db.SaveChangesAsync();
        }

        /// <inheritdoc/>
        public int Save()
        {
            return _db.SaveChanges();
        }

        /// <summary>
        /// Releases the allocated resources of this context.
        /// </summary>
        /// <param name="disposing">True if called from Dispose(); false if called from finalizer.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _db.Dispose();
                }
                _disposed = true;
            }
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
