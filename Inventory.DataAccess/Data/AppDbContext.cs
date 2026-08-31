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
        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
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
        /// Gets or sets the DbSet for application users (extended Identity users).
        /// </summary>
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;

        /// <summary>
        /// Gets or sets the DbSet for purchase orders.
        /// </summary>
        public DbSet<PurchaseOrder> PurchaseOrders { get; set; } = null!;

        /// <summary>
        /// Configures the schema needed for the application entities and Identity.
        /// </summary>
        /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure entity relationships and constraints

            // LabSupply - Supplier relationship
            modelBuilder.Entity<LabSupply>()
                .HasOne(l => l.Supplier)
                .WithMany(s => s.LabSupplies)
                .HasForeignKey(l => l.SupplierID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // PurchaseOrder - LabSupply relationship
            modelBuilder.Entity<PurchaseOrder>()
                .HasOne(p => p.LabSupply)
                .WithMany(l => l.PurchaseOrders)
                .HasForeignKey(p => p.SupplyID)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete

            // Configure indexes for better query performance
            modelBuilder.Entity<LabSupply>()
                .HasIndex(l => l.SupplyName);

            modelBuilder.Entity<Supplier>()
                .HasIndex(s => s.ContactEmail)
                .IsUnique();

            modelBuilder.Entity<PurchaseOrder>()
                .HasIndex(p => p.OrderDate);
        }
    }
}
