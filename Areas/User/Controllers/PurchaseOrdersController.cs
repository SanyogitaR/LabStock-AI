using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.Models;
using Inventory.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace InventoryManagement.Controllers
{
    /// <summary>
    /// Controller for managing purchase orders.
    /// </summary>
    [Area("User")]
    [Authorize(Roles = SD.Role_Admin_Manager_Employee)]
    public class PurchaseOrdersController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<PurchaseOrdersController> _logger;

        public PurchaseOrdersController(IUnitOfWork unitOfWork, ILogger<PurchaseOrdersController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the list of all purchase orders.
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var purchaseOrders = await _unitOfWork.PurchaseOrder.GetAllAsync(includeProperties: "LabSupply");
                return View(purchaseOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving purchase orders");
                TempData["error"] = "An error occurred while loading purchase orders.";
                return View(new List<PurchaseOrder>());
            }
        }

        /// <summary>
        /// Exports purchase orders to Excel with fulfillment tracking.
        /// </summary>
        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                
                var purchaseOrders = await _unitOfWork.PurchaseOrder.GetAllAsync(includeProperties: "LabSupply");
                
                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Purchase Orders");
                
                // Headers
                worksheet.Cells[1, 1].Value = "Order ID";
                worksheet.Cells[1, 2].Value = "Supply Name";
                worksheet.Cells[1, 3].Value = "Order Date";
                worksheet.Cells[1, 4].Value = "Quantity Ordered";
                worksheet.Cells[1, 5].Value = "Quantity Received";
                worksheet.Cells[1, 6].Value = "Quantity Remaining";
                worksheet.Cells[1, 7].Value = "Fulfillment %";
                worksheet.Cells[1, 8].Value = "Order Status";
                
                // Header styling
                using (var range = worksheet.Cells[1, 1, 1, 8])
                {
                    range.Style.Font.Bold = true;
                    range.Style.Fill.PatternType = ExcelFillStyle.Solid;
                    range.Style.Fill.BackgroundColor.SetColor(System.Drawing.Color.FromArgb(79, 129, 189));
                    range.Style.Font.Color.SetColor(System.Drawing.Color.White);
                }
                
                // Data
                int row = 2;
                foreach (var po in purchaseOrders)
                {
                    worksheet.Cells[row, 1].Value = po.OrderID;
                    worksheet.Cells[row, 2].Value = po.LabSupply?.SupplyName ?? "N/A";
                    worksheet.Cells[row, 3].Value = po.OrderDate.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 4].Value = po.QuantityOrdered;
                    worksheet.Cells[row, 5].Value = po.QuantityReceived;
                    worksheet.Cells[row, 6].Value = po.QuantityRemaining;
                    worksheet.Cells[row, 7].Value = po.FulfillmentPercentage / 100; // Convert to decimal for percentage formatting
                    worksheet.Cells[row, 7].Style.Numberformat.Format = "0.00%";
                    worksheet.Cells[row, 8].Value = po.OrderStatus;
                    
                    // Color code by status
                    var statusColor = po.IsFullyReceived ? System.Drawing.Color.LightGreen :
                                     po.QuantityReceived > 0 ? System.Drawing.Color.LightYellow :
                                     System.Drawing.Color.LightCoral;
                    worksheet.Cells[row, 8].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    worksheet.Cells[row, 8].Style.Fill.BackgroundColor.SetColor(statusColor);
                    
                    row++;
                }
                
                worksheet.Cells.AutoFitColumns();
                
                var stream = new MemoryStream(package.GetAsByteArray());
                var fileName = $"PurchaseOrders_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                
                _logger.LogInformation("Exported {Count} purchase orders to Excel", purchaseOrders.Count());
                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting purchase orders to Excel");
                TempData["error"] = "An error occurred while exporting to Excel.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Displays the details of a specific purchase order.
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["error"] = "Invalid purchase order ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var purchaseOrder = await _unitOfWork.PurchaseOrder.GetAsync(
                    p => p.OrderID == id,
                    includeProperties: "LabSupply");

                if (purchaseOrder == null)
                {
                    _logger.LogWarning("Purchase order with ID {Id} not found", id);
                    TempData["error"] = "Purchase order not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading purchase order details, ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the purchase order.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Displays the form for creating a new purchase order.
        /// </summary>
        public async Task<IActionResult> Create()
        {
            try
            {
                var labSupplies = await _unitOfWork.LabSupply.GetAllAsync();
                ViewData["SupplyID"] = new SelectList(labSupplies, "SupplyID", "SupplyName");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                TempData["error"] = "An error occurred while loading the form.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the form submission for creating a new purchase order.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PurchaseOrder purchaseOrder)
        {
            if (!ModelState.IsValid)
            {
                var labSupplies = await _unitOfWork.LabSupply.GetAllAsync();
                ViewData["SupplyID"] = new SelectList(labSupplies, "SupplyID", "SupplyName", purchaseOrder.SupplyID);
                return View(purchaseOrder);
            }

            try
            {
                // Set initial status to Pending and QuantityReceived to 0
                purchaseOrder.OrderStatus = "Pending";
                purchaseOrder.QuantityReceived = 0;
                
                await _unitOfWork.PurchaseOrder.AddAsync(purchaseOrder);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Purchase order created successfully!";
                _logger.LogInformation("Created new purchase order for SupplyID: {SupplyID}, Status: {Status}", purchaseOrder.SupplyID, purchaseOrder.OrderStatus);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating purchase order");
                TempData["error"] = "An error occurred while creating the purchase order.";

                var labSupplies = await _unitOfWork.LabSupply.GetAllAsync();
                ViewData["SupplyID"] = new SelectList(labSupplies, "SupplyID", "SupplyName", purchaseOrder.SupplyID);
                return View(purchaseOrder);
            }
        }

        /// <summary>
        /// Displays the form for editing an existing purchase order.
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["error"] = "Invalid purchase order ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var purchaseOrder = await _unitOfWork.PurchaseOrder.GetAsync(p => p.OrderID == id);

                if (purchaseOrder == null)
                {
                    _logger.LogWarning("Purchase order with ID {Id} not found", id);
                    TempData["error"] = "Purchase order not found.";
                    return RedirectToAction(nameof(Index));
                }

                var labSupplies = await _unitOfWork.LabSupply.GetAllAsync();
                ViewData["SupplyID"] = new SelectList(labSupplies, "SupplyID", "SupplyName", purchaseOrder.SupplyID);
                
                return View(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading purchase order for edit, ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the purchase order.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the form submission for updating an existing purchase order.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PurchaseOrder purchaseOrder)
        {
            if (id != purchaseOrder.OrderID)
            {
                TempData["error"] = "Invalid purchase order ID.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                var labSupplies = await _unitOfWork.LabSupply.GetAllAsync();
                ViewData["SupplyID"] = new SelectList(labSupplies, "SupplyID", "SupplyName", purchaseOrder.SupplyID);
                return View(purchaseOrder);
            }

            try
            {
                // Check if entity exists
                var existingOrder = await _unitOfWork.PurchaseOrder.GetAsync(p => p.OrderID == id);
                if (existingOrder == null)
                {
                    _logger.LogWarning("Purchase order with ID {Id} not found during update", id);
                    TempData["error"] = "Purchase order not found.";
                    return RedirectToAction(nameof(Index));
                }

                _unitOfWork.PurchaseOrder.Update(purchaseOrder);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Purchase order updated successfully!";
                _logger.LogInformation("Updated purchase order ID: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating purchase order ID: {Id}", id);
                TempData["error"] = "An error occurred while updating the purchase order.";

                var labSupplies = await _unitOfWork.LabSupply.GetAllAsync();
                ViewData["SupplyID"] = new SelectList(labSupplies, "SupplyID", "SupplyName", purchaseOrder.SupplyID);
                return View(purchaseOrder);
            }
        }

        /// <summary>
        /// Displays the confirmation page for deleting a purchase order.
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["error"] = "Invalid purchase order ID.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                var purchaseOrder = await _unitOfWork.PurchaseOrder.GetAsync(
                    p => p.OrderID == id,
                    includeProperties: "LabSupply");

                if (purchaseOrder == null)
                {
                    _logger.LogWarning("Purchase order with ID {Id} not found", id);
                    TempData["error"] = "Purchase order not found.";
                    return RedirectToAction(nameof(Index));
                }

                return View(purchaseOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading purchase order for deletion, ID: {Id}", id);
                TempData["error"] = "An error occurred while loading the purchase order.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// Handles the confirmed deletion of a purchase order.
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var purchaseOrder = await _unitOfWork.PurchaseOrder.GetAsync(p => p.OrderID == id);

                if (purchaseOrder == null)
                {
                    _logger.LogWarning("Attempted to delete non-existent purchase order ID: {Id}", id);
                    TempData["error"] = "Purchase order not found.";
                    return RedirectToAction(nameof(Index));
                }

                _unitOfWork.PurchaseOrder.Remove(purchaseOrder);
                await _unitOfWork.SaveAsync();

                TempData["success"] = "Purchase order deleted successfully!";
                _logger.LogInformation("Deleted purchase order ID: {Id}", id);

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting purchase order ID: {Id}", id);
                TempData["error"] = "An error occurred while deleting the purchase order.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
