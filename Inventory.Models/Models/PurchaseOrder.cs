using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventory.Models.Models
{
    /// <summary>
    /// Represents a purchase order for laboratory supplies.
    /// </summary>
    public class PurchaseOrder
    {
        /// <summary>
        /// Gets or sets the unique identifier for the purchase order.
        /// </summary>
        [Key]
        public int OrderID { get; set; }

        /// <summary>
        /// Gets or sets the date when the order was placed.
        /// </summary>
        [Required(ErrorMessage = "Order date is required")]
        [Display(Name = "Order Date")]
        [DataType(DataType.Date)]
        public DateTime OrderDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the quantity of items ordered.
        /// </summary>
        [Required(ErrorMessage = "Quantity ordered is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity ordered must be at least 1")]
        [Display(Name = "Quantity Ordered")]
        public int QuantityOrdered { get; set; }

        /// <summary>
        /// Gets or sets the quantity of items received so far.
        /// </summary>
        [Display(Name = "Quantity Received")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity received cannot be negative")]
        public int QuantityReceived { get; set; } = 0;

        /// <summary>
        /// Gets the remaining quantity to be received (calculated property).
        /// </summary>
        [NotMapped]
        [Display(Name = "Quantity Remaining")]
        public int QuantityRemaining => QuantityOrdered - QuantityReceived;

        /// <summary>
        /// Gets whether the order is fully received (calculated property).
        /// </summary>
        [NotMapped]
        public bool IsFullyReceived => QuantityReceived >= QuantityOrdered;

        /// <summary>
        /// Gets the fulfillment percentage (calculated property).
        /// </summary>
        [NotMapped]
        [Display(Name = "Fulfillment %")]
        public decimal FulfillmentPercentage => QuantityOrdered > 0 
            ? Math.Round((decimal)QuantityReceived / QuantityOrdered * 100, 2) 
            : 0;

        /// <summary>
        /// Gets or sets the current status of the purchase order.
        /// </summary>
        [Required(ErrorMessage = "Order status is required")]
        [StringLength(50, ErrorMessage = "Order status cannot exceed 50 characters")]
        [Display(Name = "Order Status")]
        public string OrderStatus { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the foreign key to the associated lab supply.
        /// </summary>
        [Required(ErrorMessage = "Supply selection is required")]
        [Display(Name = "Supply")]
        public int SupplyID { get; set; }

        /// <summary>
        /// Gets or sets the navigation property to the associated lab supply.
        /// </summary>
        [ForeignKey("SupplyID")]
        public LabSupply? LabSupply { get; set; }

        /// <summary>
        /// Gets the total cost of the order (calculated property).
        /// Note: Requires unit price implementation.
        /// </summary>
        [NotMapped]
        [DisplayName("Total Cost")]
        public decimal TotalCost => 0m; // Placeholder - implement when unit price is added
    }
}
