using Inventory.Models.Models;

namespace Inventory.Models.ViewModels
{
    /// <summary>
    /// View model for the dashboard displaying key inventory metrics.
    /// </summary>
    public class DashboardVM
    {
        /// <summary>
        /// Gets or sets the total number of supplies in inventory.
        /// </summary>
        public int TotalSupplies { get; set; }

        /// <summary>
        /// Gets or sets the number of supplies that need reordering.
        /// </summary>
        public int LowStockCount { get; set; }

        /// <summary>
        /// Gets or sets the number of supplies that are out of stock.
        /// </summary>
        public int OutOfStockCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of suppliers.
        /// </summary>
        public int TotalSuppliers { get; set; }

        /// <summary>
        /// Gets or sets the count of pending purchase orders.
        /// </summary>
        public int PendingOrdersCount { get; set; }

        /// <summary>
        /// Gets or sets the count of completed purchase orders.
        /// </summary>
        public int CompletedOrdersCount { get; set; }

        /// <summary>
        /// Gets or sets the list of supplies that need reordering.
        /// </summary>
        public List<LabSupply> LowStockSupplies { get; set; } = new();

        /// <summary>
        /// Gets or sets the list of recent purchase orders.
        /// </summary>
        public List<PurchaseOrder> RecentOrders { get; set; } = new();

        /// <summary>
        /// Gets or sets the estimated total inventory value.
        /// </summary>
        public decimal EstimatedInventoryValue { get; set; }
    }
}
