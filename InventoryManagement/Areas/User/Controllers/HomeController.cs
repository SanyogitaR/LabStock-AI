using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace InventoryManagement.Areas.User.Controllers
{
    /// <summary>
    /// Controller for public-facing pages in the User area.
    /// </summary>
    [Area("User")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        /// <summary>
        /// Displays the catalog of available lab supplies.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var supplyList = await _unitOfWork.LabSupply.GetAllAsync(includeProperties: "Supplier");
                return View(supplyList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lab supplies for catalog");
                TempData["error"] = "An error occurred while loading the catalog.";
                return View(new List<LabSupply>());
            }
        }

        /// <summary>
        /// Displays detailed information about a specific lab supply.
        /// </summary>
        /// <param name="id">The ID of the lab supply.</param>
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var supply = await _unitOfWork.LabSupply.GetAsync(
                    u => u.SupplyID == id,
                    includeProperties: "Supplier");

                if (supply == null)
                {
                    _logger.LogWarning("Lab supply with ID {Id} not found", id);
                    TempData["error"] = "Lab supply not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(supply);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading lab supply details, ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the supply details.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Displays the privacy policy page.
        /// </summary>
        public IActionResult Privacy()
        {
            return View();
        }

        /// <summary>
        /// Displays the error page.
        /// </summary>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}