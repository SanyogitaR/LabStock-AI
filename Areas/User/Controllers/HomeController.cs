using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
namespace InventoryManagement.Areas.User.Controllers
{
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        public HomeController(
            ILogger<HomeController> logger,
            IUnitOfWork unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
        public async Task<IActionResult> Index()
        {
            try
            {
                var supplyList = await _unitOfWork.LabSupply.GetAllAsync(
                    includeProperties: "Supplier");
                ViewBag.StockInsight = GetStockInsight(supplyList.ToList());
                return View(supplyList);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving lab supplies for catalog");
                TempData["error"] =
                    "An error occurred while loading the catalog.";
                return View(new List<LabSupply>());
            }
        }
        private string GetStockInsight(List<LabSupply> supplies)
        {
            var lowStock = supplies.Where(s => s.NeedsReorder).ToList();
            if (lowStock.Count == 0)
                return "All lab supplies are currently at healthy stock levels.";
            if (lowStock.Count <= 2)
                return $"{lowStock.Count} item(s) need attention soon: {string.Join(", ", lowStock.Select(s => s.SupplyName))}.";
            return $"Multiple supplies are critically low ({lowStock.Count} items). Immediate reordering is recommended.";
        }
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var supply = await _unitOfWork.LabSupply.GetAsync(
                    u => u.SupplyID == id,
                    includeProperties: "Supplier");
                if (supply == null)
                {
                    _logger.LogWarning(
                        "Lab supply with ID {Id} not found",
                        id);
                    TempData["error"] = "Lab supply not found.";
                    return RedirectToAction(nameof(Index));
                }
                return View(supply);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error loading lab supply details, ID: {Id}",
                    id);
                TempData["error"] =
                    "An error occurred while loading the supply details.";
                return RedirectToAction(nameof(Index));
            }
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(
                new InventoryManagement.Models.ErrorViewModel
                {
                    RequestId = Activity.Current?.Id
                                ?? HttpContext.TraceIdentifier
                });
        }
    }
}
