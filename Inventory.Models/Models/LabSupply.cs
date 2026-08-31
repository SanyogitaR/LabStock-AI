using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models.Models
{
    /// <summary>
    /// Represents a laboratory supply item in the inventory system.
    /// </summary>
    public class LabSupply
    {
        /// <summary>
        /// Gets or sets the unique identifier for the lab supply.
        /// </summary>
        [Key]
        public int SupplyID { get; set; }

        /// <summary>
        /// Gets or sets the name of the lab supply.
        /// </summary>
        [Required(ErrorMessage = "Supply name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Supply name must be between 2 and 100 characters")]
        [Display(Name = "Supply Name")]
        public string SupplyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current quantity available in inventory.
        /// </summary>
        [Required(ErrorMessage = "Quantity on hand is required")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity on hand must be zero or greater")]
        [Display(Name = "Quantity on Hand")]
        public int QuantityOnHand { get; set; }

        /// <summary>
        /// Gets or sets the minimum quantity threshold that triggers a reorder alert.
        /// </summary>
        [Required(ErrorMessage = "Reorder point is required")]
        [Display(Name = "Reorder Point")]
        [Range(1, int.MaxValue, ErrorMessage = "Reorder point must be a positive value")]
        public int ReorderPoint { get; set; }

        /// <summary>
        /// Gets or sets the relative URL path to the supply's image.
        /// </summary>
        [ValidateNever]
        [StringLength(500, ErrorMessage = "Image URL cannot exceed 500 characters")]
        public string? ImageURL { get; set; }

        /// <summary>
        /// Gets or sets the foreign key to the associated supplier.
        /// </summary>
        [Required(ErrorMessage = "Supplier selection is required")]
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated supplier.
        /// </summary>
        [ForeignKey("SupplierID")]
        [ValidateNever]
        public Supplier? Supplier { get; set; }

        /// <summary>
        /// Gets a value indicating whether this supply needs to be reordered.
        /// </summary>
        [NotMapped]
        public bool NeedsReorder => QuantityOnHand <= ReorderPoint;

        /// <summary>
        /// Gets or sets the collection of purchase orders associated with this supply.
        /// </summary>
        [ValidateNever]
        public ICollection<PurchaseOrder>? PurchaseOrders { get; set; }
    }
}
