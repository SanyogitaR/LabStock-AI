using Inventory.Models.Models;

namespace Inventory.DataAccess.Repository.IRepository
{
    /// <summary>
    /// Repository interface for LabSupply-specific data operations.
    /// Inherits the standard CRUD surface from the generic repository.
    /// </summary>
    public interface ILabSupplyRepository : IRepository<LabSupply>
    {
        void Update(LabSupply labSupply);
    }
}
