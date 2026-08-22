using Inventory.Models.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Inventory.Models.ViewModels
{
    /// <summary>
    /// View model backing the LabSupplies Upsert (create/edit) view.
    /// </summary>
    public class LabSupplyVM
    {
        public LabSupply LabSupply { get; set; } = new LabSupply();

        public IEnumerable<SelectListItem> SupplierList { get; set; } = new List<SelectListItem>();
    }
}
