using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Inventory.Models.Models
{
    /// <summary>
    /// Represents a supplier that provides laboratory supplies.
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// Gets or sets the unique identifier for the supplier.
        /// </summary>
        [Key]
        public int SupplierID { get; set; }

        /// <summary>
        /// Gets or sets the name of the supplier company.
        /// </summary>
        [Required(ErrorMessage = "Supplier name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Supplier name must be between 2 and 100 characters")]
        [DisplayName("Supplier Name")]
        public string SupplierName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the primary contact person at the supplier.
        /// </summary>
        [Required(ErrorMessage = "Contact person is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Contact person name must be between 2 and 100 characters")]
        [DisplayName("Contact Person")]
        public string ContactPerson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email address for the supplier contact.
        /// </summary>
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [DisplayName("Contact Email")]
        public string ContactEmail { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the collection of lab supplies provided by this supplier.
        /// </summary>
        public ICollection<LabSupply>? LabSupplies { get; set; }
    }
}
