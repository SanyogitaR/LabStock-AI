using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.ViewModels;
using Inventory.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for the admin dashboard with key inventory metrics.
    /// </summary>
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(IUnitOfWork unitOfWork, ILogger<DashboardController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the admin dashboard with key metrics and insights.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var dashboardVM = new DashboardVM();

                // Get all supplies
                var allSupplies = await _unitOfWork.LabSupply.GetAllAsync(includeProperties: "Supplier");
                var suppliesList = allSupplies.ToList();

                // Calculate metrics
                dashboardVM.TotalSupplies = suppliesList.Count;
                dashboardVM.LowStockCount = suppliesList.Count(s => s.NeedsReorder && s.QuantityOnHand > 0);
                dashboardVM.OutOfStockCount = suppliesList.Count(s => s.QuantityOnHand == 0);
                
                // Get suppliers count
                var suppliers = await _unitOfWork.Supplier.GetAllAsync();
                dashboardVM.TotalSuppliers = suppliers.Count();

                // Get purchase orders
                var allOrders = await _unitOfWork.PurchaseOrder.GetAllAsync(includeProperties: "LabSupply");
                var ordersList = allOrders.ToList();

                dashboardVM.PendingOrdersCount = ordersList.Count(o => o.OrderStatus.Equals("Pending", StringComparison.OrdinalIgnoreCase));
                dashboardVM.CompletedOrdersCount = ordersList.Count(o => o.OrderStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) || 
                                                                          o.OrderStatus.Equals("Received", StringComparison.OrdinalIgnoreCase));

                // Get low stock supplies (top 5)
                dashboardVM.LowStockSupplies = suppliesList
                    .Where(s => s.NeedsReorder)
                    .OrderBy(s => s.QuantityOnHand)
                    .Take(5)
                    .ToList();

                // Get recent orders (top 5)
                dashboardVM.RecentOrders = ordersList
                    .OrderByDescending(o => o.OrderDate)
                    .Take(5)
                    .ToList();

                _logger.LogInformation("Dashboard loaded successfully");
                return View(dashboardVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading dashboard");
                TempData["error"] = "An error occurred while loading the dashboard.";
                return View(new DashboardVM());
            }
        }
    }
}
