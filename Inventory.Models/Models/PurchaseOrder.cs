using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models.Models
{
    public class PurchaseOrder
    {
        [Key]
        public int OrderID { get; set; }

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

        public string OrderStatus { get; set; } = "Pending";

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [NotMapped]
        public int QuantityRemaining
        {
            get
            {
                return Math.Max(0, QuantityOrdered - QuantityReceived);
            }
        }

        [NotMapped]
        public decimal FulfillmentPercentage
        {
            get
            {
                if (QuantityOrdered <= 0)
                    return 0;

                return (decimal)QuantityReceived / QuantityOrdered * 100;
            }
        }

        [NotMapped]
        public bool IsFullyReceived
        {
            get
            {
                return QuantityReceived >= QuantityOrdered;
            }
        }

        [NotMapped]
        public string Status
        {
            get => OrderStatus;
            set => OrderStatus = value;
        }
    }
}