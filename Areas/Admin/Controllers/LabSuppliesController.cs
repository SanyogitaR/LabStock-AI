using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Inventory.Models.ViewModels;
using Inventory.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace InventoryManagement.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing laboratory supplies in the admin area.
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin_Manager)]
    public class LabSuppliesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<LabSuppliesController> _logger;

        public LabSuppliesController(
            IUnitOfWork unitOfWork, 
            IWebHostEnvironment webHostEnvironment,
            ILogger<LabSuppliesController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _webHostEnvironment = webHostEnvironment ?? throw new ArgumentNullException(nameof(webHostEnvironment));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the list of all lab supplies.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var labSupplyList = await _unitOfWork.LabSupply.GetAllAsync(includeProperties: "Supplier");
                return View(labSupplyList);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lab supplies");
                TempData["error"] = "An error occurred while loading lab supplies.";
                return View(new List<LabSupply>());
            }
        }
      

        /// <summary>
        /// Displays the form for creating or updating a lab supply.
        /// </summary>
        /// <param name="id">The ID of the lab supply to edit, or null for creation.</param>
        public async Task<IActionResult> Upsert(int? id)
        {
            try
            {
                var suppliers = await _unitOfWork.Supplier.GetAllAsync();
                
                var labSupplyVM = new LabSupplyVM
                {
                    SupplierList = suppliers.Select(u => new SelectListItem
                    {
                        Text = u.SupplierName,
                        Value = u.SupplierID.ToString()
                    }),
                    LabSupply = new LabSupply()
                };

                if (id == null || id == 0)
                {
                    // Create mode
                    return View(labSupplyVM);
                }

                // Update mode
                labSupplyVM.LabSupply = await _unitOfWork.LabSupply.GetAsync(u => u.SupplyID == id);
                
                if (labSupplyVM.LabSupply == null)
                {
                    _logger.LogWarning("Lab supply with ID {Id} not found", id);
                    TempData["error"] = "Lab supply not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(labSupplyVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading upsert form for lab supply ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the form submission for creating or updating a lab supply.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(LabSupplyVM labSupplyVM, IFormFile? file)
        {
            // Remove ImageURL from validation
            ModelState.Remove("LabSupply.ImageURL");

            if (!ModelState.IsValid)
            {
                // Repopulate supplier list
                var suppliers = await _unitOfWork.Supplier.GetAllAsync();
                labSupplyVM.SupplierList = suppliers.Select(u => new SelectListItem
                {
                    Text = u.SupplierName,
                    Value = u.SupplierID.ToString()
                });
                return View(labSupplyVM);
            }

            try
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                
                // Handle image upload
                if (file != null)
                {
                    // Validate file type
                    string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                    
                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        TempData["error"] = "Invalid file type. Only JPG, PNG, and GIF images are allowed.";
                        return await RepopulateAndReturn(labSupplyVM);
                    }

                    // Validate file size (5MB max)
                    if (file.Length > 5 * 1024 * 1024)
                    {
                        TempData["error"] = "File size cannot exceed 5MB.";
                        return await RepopulateAndReturn(labSupplyVM);
                    }

                    string fileName = Guid.NewGuid().ToString() + fileExtension;
                    string productPath = Path.Combine(wwwRootPath, "images", "supply");

                    // Ensure directory exists
                    if (!Directory.Exists(productPath))
                    {
                        Directory.CreateDirectory(productPath);
                    }

                    // Delete old image if updating
                    if (!string.IsNullOrEmpty(labSupplyVM.LabSupply.ImageURL))
                    {
                        var oldImagePath = Path.Combine(wwwRootPath, labSupplyVM.LabSupply.ImageURL.TrimStart('\\', '/'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            try
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                            catch (Exception ex)
                            {
                                _logger.LogWarning(ex, "Failed to delete old image: {Path}", oldImagePath);
                            }
                        }
                    }

                    // Upload new image
                    string filePath = Path.Combine(productPath, fileName);
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    // Update ImageURL with forward slashes
                    labSupplyVM.LabSupply.ImageURL = "/images/supply/" + fileName;
                }
                else if (labSupplyVM.LabSupply.SupplyID == 0)
                {
                    // Set default image for new supplies without uploaded image
                    labSupplyVM.LabSupply.ImageURL = "/images/supply/default.jpg";
                }

                // Add or update the supply
                if (labSupplyVM.LabSupply.SupplyID == 0)
                {
                    await _unitOfWork.LabSupply.AddAsync(labSupplyVM.LabSupply);
                    TempData["success"] = "Lab supply created successfully!";
                    _logger.LogInformation("Created new lab supply: {Name}", labSupplyVM.LabSupply.SupplyName);
                }
                else
                {
                    _unitOfWork.LabSupply.Update(labSupplyVM.LabSupply);
                    TempData["success"] = "Lab supply updated successfully!";
                    _logger.LogInformation("Updated lab supply ID: {Id}", labSupplyVM.LabSupply.SupplyID);
                }

                await _unitOfWork.SaveAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving lab supply");
                TempData["error"] = "An error occurred while saving the lab supply.";
                return await RepopulateAndReturn(labSupplyVM);
            }
        }

        /// <summary>
        /// Helper method to repopulate ViewModel and return the view.
        /// </summary>
        private async Task<IActionResult> RepopulateAndReturn(LabSupplyVM labSupplyVM)
        {
            var suppliers = await _unitOfWork.Supplier.GetAllAsync();
            labSupplyVM.SupplierList = suppliers.Select(u => new SelectListItem
            {
                Text = u.SupplierName,
                Value = u.SupplierID.ToString()
            });
            return View(labSupplyVM);
        }

        //// GET: LabSupplies/Edit/5
        //public IActionResult Edit(int? id)
        //{

        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    LabSupply? labSupplyFromDb = _unitOfWork.LabSupply.Get(u => u.SupplyID == id);

        //    if (labSupplyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(labSupplyFromDb);
        //}

        //// POST: LabSupplies/Edit/5
        //[HttpPost]
        //public IActionResult Edit(LabSupply obj)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.LabSupply.update(obj);
        //        _unitOfWork.Save();
        //        TempData["success"] = "LabSupply Updated Successfully";
        //        return RedirectToAction("Index");
        //    }
        //    return View();
        //}



        ///// GET: LabSupplies/Delete/5
        //public IActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }
        //    LabSupply? labSupplyFromDb = _unitOfWork.LabSupply.Get(u => u.SupplyID == id);

        //    if (labSupplyFromDb == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(labSupplyFromDb);

        //}

        //// POST: LabSupplies/Delete/5
        //[HttpPost, ActionName("Delete")]
        //public IActionResult DeletePOST(int? id)
        //{
        //    LabSupply? obj = _unitOfWork.LabSupply.Get(u => u.SupplyID == id);
            
        //    if (obj == null)
        //    {
        //        return NotFound();
        //    }

        //    _unitOfWork.LabSupply.Remove(obj);
        //    _unitOfWork.Save();
        //    TempData["success"] = "LabSupply Deleted Successfully";
        //    return RedirectToAction("Index");            
        //}

        #region API CALLS

        /// <summary>
        /// API endpoint to get all lab supplies for DataTables.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var labSupplyList = await _unitOfWork.LabSupply.GetAllAsync(includeProperties: "Supplier");

                var jsonOptions = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    PropertyNamingPolicy = null
                };

                var jsonData = JsonSerializer.Serialize(new { data = labSupplyList }, jsonOptions);
                return Content(jsonData, "application/json");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lab supplies for DataTable");
                return Json(new { data = new List<LabSupply>() });
            }
        }

        /// <summary>
        /// API endpoint to delete a lab supply.
        /// </summary>
        [HttpDelete]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Invalid ID" });
            }

            try
            {
                var supplyToBeDeleted = await _unitOfWork.LabSupply.GetAsync(u => u.SupplyID == id);
                
                if (supplyToBeDeleted == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent lab supply ID: {Id}", id);
                    return Json(new { success = false, message = "Lab supply not found" });
                }

                // Delete associated image if exists
                if (!string.IsNullOrEmpty(supplyToBeDeleted.ImageURL) && 
                    supplyToBeDeleted.ImageURL != "/images/supply/default.jpg")
                {
                    var imagePath = Path.Combine(
                        _webHostEnvironment.WebRootPath,
                        supplyToBeDeleted.ImageURL.TrimStart('\\', '/'));

                    if (System.IO.File.Exists(imagePath))
                    {
                        try
                        {
                            System.IO.File.Delete(imagePath);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogWarning(ex, "Failed to delete image file: {Path}", imagePath);
                        }
                    }
                }

                _unitOfWork.LabSupply.Remove(supplyToBeDeleted);
                await _unitOfWork.SaveAsync();

                _logger.LogInformation("Deleted lab supply ID: {Id}, Name: {Name}", id, supplyToBeDeleted.SupplyName);
                return Json(new { success = true, message = "Lab supply deleted successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting lab supply ID: {Id}", id);
                return Json(new { success = false, message = "An error occurred while deleting the lab supply" });
            }
        }

        #endregion
    }
}


