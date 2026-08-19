using Inventory.DataAccess.Repository.IRepository;
using Inventory.Models.ViewModels;
using Inventory.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryManagement.Areas.Admin.Controllers
{
    /// <summary>
    /// Controller for managing stock adjustments (adding/removing inventory).
    /// </summary>
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin_Manager_Employee)]
    public class StockAdjustmentController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<StockAdjustmentController> _logger;

        public StockAdjustmentController(IUnitOfWork unitOfWork, ILogger<StockAdjustmentController> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Displays the stock adjustment form.
        /// </summary>
        public async Task<IActionResult> Index(int? id)
        {
            try
            {
                if (id == null)
                {
                    // Show supply selection
                    var supplies = await _unitOfWork.LabSupply.GetAllAsync(includeProperties: "Supplier");
                    ViewBag.Supplies = supplies;
                    return View();
                }

                // Load specific supply for adjustment
                var supply = await _unitOfWork.LabSupply.GetAsync(s => s.SupplyID == id, includeProperties: "Supplier");
                
                if (supply == null)
                {
                    TempData["error"] = "Supply not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Get pending purchase orders for this supply
                var allPurchaseOrders = await _unitOfWork.PurchaseOrder.GetAllAsync(includeProperties: "LabSupply");
                
                var purchaseOrders = allPurchaseOrders
                    .Where(po => po.SupplyID == id && 
                                 !po.IsFullyReceived)
                    .ToList();
                
                ViewBag.PurchaseOrders = purchaseOrders;

                var adjustmentVM = new StockAdjustmentVM
                {
                    SupplyID = supply.SupplyID,
                    SupplyName = supply.SupplyName,
                    CurrentQuantity = supply.QuantityOnHand
                };

                return View("Adjust", adjustmentVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading stock adjustment form");
                TempData["error"] = "An error occurred while loading the form.";
                return RedirectToAction("Index", "Dashboard");
            }
        }

        /// <summary>
        /// Processes the stock adjustment.
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Adjust(StockAdjustmentVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {

                var supply = await _unitOfWork.LabSupply.GetAsync(s => s.SupplyID == model.SupplyID);
                
                if (supply == null)
                {
                    TempData["error"] = "Supply not found.";
                    return RedirectToAction(nameof(Index));
                }

                int oldQuantity = supply.QuantityOnHand;

                if (model.AdjustmentType == "Add")
                {
                    supply.QuantityOnHand += model.Quantity;
                    
                    // If this is a purchase order receipt, update the PO
                    if (model.PurchaseOrderID.HasValue && model.PurchaseOrderID.Value > 0)
                    {
                        var purchaseOrder = await _unitOfWork.PurchaseOrder.GetAsync(po => po.OrderID == model.PurchaseOrderID.Value);
                        
                        if (purchaseOrder != null)
                        {
                            // Check if receiving more than ordered
                            if (purchaseOrder.QuantityReceived + model.Quantity > purchaseOrder.QuantityOrdered)
                            {
                                TempData["error"] = $"Cannot receive more than ordered. Ordered: {purchaseOrder.QuantityOrdered}, Already Received: {purchaseOrder.QuantityReceived}";
                                model.CurrentQuantity = supply.QuantityOnHand;
                                model.SupplyName = supply.SupplyName;
                                return View(model);
                            }
                            
                            purchaseOrder.QuantityReceived += model.Quantity;
                            
                            // Auto-update status based on fulfillment
                            if (purchaseOrder.IsFullyReceived)
                            {
                                purchaseOrder.OrderStatus = "Received";
                            }
                            else if (purchaseOrder.QuantityReceived > 0)
                            {
                                purchaseOrder.OrderStatus = "Partially Received";
                            }
                            
                            _unitOfWork.PurchaseOrder.Update(purchaseOrder);
                        }
                    }
                }
                else if (model.AdjustmentType == "Remove")
                {
                    if (supply.QuantityOnHand < model.Quantity)
                    {
                        TempData["error"] = "Cannot remove more items than available in stock.";
                        model.CurrentQuantity = supply.QuantityOnHand;
                        model.SupplyName = supply.SupplyName;
                        return View(model);
                    }
                    supply.QuantityOnHand -= model.Quantity;
                }

                _unitOfWork.LabSupply.Update(supply);
                await _unitOfWork.SaveAsync();

                string successMessage = $"Successfully {(model.AdjustmentType == "Add" ? "added" : "removed")} {model.Quantity} units. New quantity: {supply.QuantityOnHand}";
                if (model.PurchaseOrderID.HasValue && model.PurchaseOrderID.Value > 0)
                {
                    successMessage += $" | Purchase Order #{model.PurchaseOrderID.Value} updated.";
                }
                TempData["success"] = successMessage;
                
                return RedirectToAction("Index", "LabSupplies");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing stock adjustment");
                TempData["error"] = "An error occurred while adjusting stock.";
                return View(model);
            }
        }
    }
}
