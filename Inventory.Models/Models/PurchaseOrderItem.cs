using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models.Models
{
    public class PurchaseOrderItem
    {
        [Key]
        public int OrderItemID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [ForeignKey("OrderID")]
        public PurchaseOrder? PurchaseOrder { get; set; }

        [Required]
        public int SupplyID { get; set; }

        [ForeignKey("SupplyID")]
        public LabSupply? LabSupply { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int QuantityOrdered { get; set; }

        [Range(0, int.MaxValue)]
        public int QuantityReceived { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitPrice { get; set; }
    }
}
