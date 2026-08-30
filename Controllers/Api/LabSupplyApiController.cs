using Microsoft.AspNetCore.Mvc;
using Inventory.DataAccess.Repository.IRepository;

namespace InventoryManagement.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class LabSupplyApiController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public LabSupplyApiController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("lowstock")]
        public async Task<IActionResult> GetLowStock()
        {
            var supplies = await _unitOfWork.LabSupply.GetAllAsync();
            var lowStock = supplies.Where(s => s.NeedsReorder)
                .Select(s => new { s.SupplyName, s.QuantityOnHand, s.ReorderPoint });
            return Ok(lowStock);
        }
    }
}
