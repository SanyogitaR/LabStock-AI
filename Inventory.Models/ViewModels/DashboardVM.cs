using Inventory.Models.Models;

namespace Inventory.Models.ViewModels
{
   
    public class DashboardVM
    {
        
        public int TotalSupplies { get; set; }

        
        public int LowStockCount { get; set; }

        
        public int OutOfStockCount { get; set; }

       
        public int TotalSuppliers { get; set; }

       
        public int PendingOrdersCount { get; set; }

        
        public int CompletedOrdersCount { get; set; }

       
        public List<LabSupply> LowStockSupplies { get; set; } = new();

        
        public List<PurchaseOrder> RecentOrders { get; set; } = new();

        
        public decimal EstimatedInventoryValue { get; set; }
    }
}
