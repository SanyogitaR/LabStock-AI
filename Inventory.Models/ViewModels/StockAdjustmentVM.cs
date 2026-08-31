using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.ViewModels
{
    /// <summary>
    /// View model for adjusting inventory stock levels.
    /// </summary>
    public class StockAdjustmentVM
    {
        /// <summary>
        /// Gets or sets the supply ID being adjusted.
        /// </summary>
        public int SupplyID { get; set; }

        /// <summary>
        /// Gets or sets the supply name for display.
        /// </summary>
        [Display(Name = "Supply Name")]
        public string SupplyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the current quantity on hand.
        /// </summary>
        [Display(Name = "Current Quantity")]
        public int CurrentQuantity { get; set; }

        /// <summary>
        /// Gets or sets the type of adjustment (Add or Remove).
        /// </summary>
        [Required(ErrorMessage = "Adjustment type is required")]
        [Display(Name = "Adjustment Type")]
        public string AdjustmentType { get; set; } = "Add";

        /// <summary>
        /// Gets or sets the quantity to adjust.
        /// </summary>
        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, 10000, ErrorMessage = "Quantity must be between 1 and 10000")]
        [Display(Name = "Quantity")]
        public int Quantity { get; set; }

        /// <summary>
        /// Gets or sets the reason for the adjustment.
        /// </summary>
        [Required(ErrorMessage = "Reason is required")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters")]
        [Display(Name = "Reason")]
        public string Reason { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the reference number (e.g., invoice, damage report).
        /// </summary>
        [StringLength(100, ErrorMessage = "Reference cannot exceed 100 characters")]
        [Display(Name = "Reference #")]
        public string? Reference { get; set; }

        /// <summary>
        /// Gets or sets the Purchase Order ID if this adjustment is receiving PO items.
        /// </summary>
        [Display(Name = "Purchase Order")]
        public int? PurchaseOrderID { get; set; }
    }
}
