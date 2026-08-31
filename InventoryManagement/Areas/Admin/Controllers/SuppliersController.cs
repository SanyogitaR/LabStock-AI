using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Inventory.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing suppliers in the admin area.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin_Manager)]
    public class SuppliersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SuppliersController> _logger;

        public SuppliersController(IUnitOfWork unitOfWork, ILogger<SuppliersController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the list of all suppliers.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var suppliersList = await _unitOfWork.Supplier.GetAllAsync();
                return View(suppliersList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving suppliers");
                TempData["error"] = "An error occurred while loading suppliers.";
                return View(new List<Supplier>());
            }
        }

        /// <summary>
        /// Displays the form for creating a new supplier.
        /// </summary>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// Handles the form submission for creating a new supplier.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Supplier obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            try
            {
                await _unitOfWork.Supplier.AddAsync(obj);
                await _unitOfWork.SaveAsync();
                
                TempData["success"] = "Supplier created successfully!";
                _logger.LogInformation("Created new supplier: {Name}", obj.SupplierName);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating supplier");
                TempData["error"] = "An error occurred while creating the supplier.";
                return View(obj);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing supplier.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["error"] = "Invalid supplier ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var supplierFromDb = await _unitOfWork.Supplier.GetAsync(u => u.SupplierID == id);

                if (supplierFromDb == null)
                {
                    _logger.LogWarning("Supplier with ID {Id} not found", id);
                    TempData["error"] = "Supplier not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(supplierFromDb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading supplier for edit, ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the supplier.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the form submission for updating an existing supplier.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Supplier obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            try
            {
                _unitOfWork.Supplier.Update(obj);
                await _unitOfWork.SaveAsync();
                
                TempData["success"] = "Supplier updated successfully!";
                _logger.LogInformation("Updated supplier ID: {Id}", obj.SupplierID);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating supplier ID: {Id}", obj.SupplierID);
                TempData["error"] = "An error occurred while updating the supplier.";
                return View(obj);
            }
        }

        /// <summary>
        /// Displays the confirmation page for deleting a supplier.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["error"] = "Invalid supplier ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var supplierFromDb = await _unitOfWork.Supplier.GetAsync(u => u.SupplierID == id);

                if (supplierFromDb == null)
                {
                    _logger.LogWarning("Supplier with ID {Id} not found", id);
                    TempData["error"] = "Supplier not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(supplierFromDb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading supplier for deletion, ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the supplier.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the confirmed deletion of a supplier.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int? id)
        {
            if (id == null)
            {
                TempData["error"] = "Invalid supplier ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var obj = await _unitOfWork.Supplier.GetAsync(u => u.SupplierID == id);
                
                if (obj == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent supplier ID: {Id}", id);
                    TempData["error"] = "Supplier not found.";
                    return RedirectToAction(nameof(Index));
                }

                _unitOfWork.Supplier.Remove(obj);
                await _unitOfWork.SaveAsync();
                
                TempData["success"] = "Supplier deleted successfully!";
                _logger.LogInformation("Deleted supplier ID: {Id}, Name: {Name}", id, obj.SupplierName);
                
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting supplier ID: {Id}", id);
                TempData["error"] = "An error occurred while deleting the supplier. It may be referenced by other records.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
