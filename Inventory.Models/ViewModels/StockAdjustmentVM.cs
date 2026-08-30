using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.ViewModels
{
    public class StockAdjustmentVM
    {
        public int SupplyID { get; set; }

        public string SupplyName { get; set; } = string.Empty;

        public int CurrentQuantity { get; set; }

        [Required(ErrorMessage = "Please select an adjustment type.")]
        public string AdjustmentType { get; set; } = "Add";

        [Required(ErrorMessage = "Please enter a quantity.")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Please select or enter a reason.")]
        public string Reason { get; set; } = string.Empty;

        public int? PurchaseOrderID { get; set; }

        public string? Reference { get; set; }
    }
}