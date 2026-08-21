using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models.Models
{
    /// <summary>
    /// Represents a laboratory supply item tracked in inventory.
    /// </summary>
    public class LabSupply
    {
        [Key]
        public int SupplyID { get; set; }

        [Required(ErrorMessage = "Supply name is required")]
        [StringLength(100)]
        [Display(Name = "Supply Name")]
        public string SupplyName { get; set; } = string.Empty;

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        [Display(Name = "Quantity On Hand")]
        public int QuantityOnHand { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Reorder point cannot be negative")]
        [Display(Name = "Reorder Point")]
        public int ReorderPoint { get; set; }

        [Display(Name = "Image")]
        public string? ImageURL { get; set; }

        [Required(ErrorMessage = "Supplier is required")]
        [Display(Name = "Supplier")]
        public int SupplierID { get; set; }

        [ForeignKey("SupplierID")]
        public Supplier? Supplier { get; set; }
    }
}
