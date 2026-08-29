namespace Inventory.DataAccess.Repository.IRepository
{
    /// <summary>
    /// Unit of Work interface for managing repository instances and database transactions.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Gets the repository for Supplier entities.
        /// </summary>
        ISupplierRepository Supplier { get; }

        /// <summary>
        /// Gets the repository for LabSupply entities.
        /// </summary>
        ILabSupplyRepository LabSupply { get; }

        /// <summary>
        /// Gets the repository for PurchaseOrder entities.
        /// </summary>
        IPurchaseOrderRepository PurchaseOrder { get; }

        /// <summary>
        /// Saves all changes made in the current unit of work to the database.
        /// </summary>
        /// <returns>A task that represents the asynchronous save operation.</returns>
        Task<int> SaveAsync();

        /// <summary>
        /// Synchronously saves all changes made in the current unit of work to the database.
        /// Use <see cref="SaveAsync"/> for asynchronous operations when possible.
        /// </summary>
        /// <returns>The number of state entries written to the database.</returns>
        int Save();
    }
}
