using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            // Dashboard summary data
            ViewBag.TotalSupplies = 125;
            ViewBag.LowStock = 8;
            ViewBag.TotalSuppliers = 17;
            ViewBag.PendingOrders = 5;

            return View();
        }
    }
}
