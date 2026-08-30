using Inventory.DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var supplies = await _context.LabSupplies
                .Include(s => s.Supplier)
                .AsNoTracking()
                .ToListAsync();

            int totalSupplies = supplies.Count;

            int outOfStockItems = supplies.Count(s =>
                s.QuantityOnHand == 0);

            int lowStockItems = supplies.Count(s =>
                s.QuantityOnHand > 0 &&
                s.QuantityOnHand <= s.ReorderPoint);

            var suppliers = await _context.Suppliers
                .AsNoTracking()
                .ToListAsync();

            int totalSuppliers = suppliers.Count;

            var lowStockSupplies = supplies
                .Where(s => s.QuantityOnHand <= s.ReorderPoint)
                .OrderBy(s => s.QuantityOnHand)
                .Take(5)
                .ToList();

            int pendingOrders = await _context.PurchaseOrders
                .CountAsync(p => p.Status == "Pending");

            int completedOrders = await _context.PurchaseOrders
                .CountAsync(p => p.Status == "Completed");

            ViewBag.TotalSupplies = totalSupplies;
            ViewBag.LowStockItems = lowStockItems;
            ViewBag.OutOfStockItems = outOfStockItems;
            ViewBag.TotalSuppliers = totalSuppliers;
            ViewBag.LowStockSupplies = lowStockSupplies;
            ViewBag.PendingOrders = pendingOrders;
            ViewBag.CompletedOrders = completedOrders;

            return View();
        }
    }
}