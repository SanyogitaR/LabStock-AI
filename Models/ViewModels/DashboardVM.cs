namespace InventoryManagement.Models.ViewModels
{
    public class DashboardVM
    {
        public int TotalSupplies { get; set; }

        public int LowStockItems { get; set; }

        public int OutOfStockItems { get; set; }

        public int TotalSuppliers { get; set; }

        public int PendingOrders { get; set; }

        public int CompletedOrders { get; set; }
    }
}