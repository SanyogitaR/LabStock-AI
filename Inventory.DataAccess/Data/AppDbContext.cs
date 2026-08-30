using Inventory.Models;
using Inventory.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Inventory.DataAccess.Data
{
    /// <summary>
    /// Application database context that manages entity sets and database operations.
    /// Inherits from IdentityDbContext to support ASP.NET Core Identity.
    /// </summary>
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets or sets the DbSet for laboratory supplies.
        /// </summary>
        public DbSet<LabSupply> LabSupplies { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for suppliers.
        /// </summary>
        public DbSet<Supplier> Suppliers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for application users.
        /// </summary>
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for purchase orders.
        /// </summary>
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for purchase order items.
        /// </summary>
        public DbSet<PurchaseOrderItem> PurchaseOrderItems { get; set; } = null!;
    }
}