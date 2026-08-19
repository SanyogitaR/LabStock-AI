/**
 * Lab Supplies DataTable Management
 * Handles loading, displaying, and deleting lab supply data
 */

let dataTable;

// Initialize DataTable when DOM is ready
$(document).ready(function () {
    loadDataTable();
});

/**
 * Loads lab supplies data from the server and initializes DataTable
 * @async
 */
async function loadDataTable() {
    try {
        const response = await fetch('/admin/LabSupplies/getall', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        if (!response.ok) {
            throw new Error(`HTTP error! status: ${response.status}`);
        }

        const data = await response.json();
        console.log("Received JSON data:", data);

        // Process nested data structure from JSON serialization with ReferenceHandler.Preserve
        const labSupplies = flattenLabSuppliesData(data.data);

        // Initialize DataTable with processed data
        initializeDataTable(labSupplies);

        console.log("DataTable initialized successfully!");
    } catch (error) {
        console.error("Error fetching lab supplies data:", error);
        toastr.error('Failed to load lab supplies. Please refresh the page.');
    }
}

/**
 * Flattens nested lab supplies data structure
 * @param {Object} dataWithValues - Data object with $values property
 * @returns {Array} Flattened array of lab supplies
 */
function flattenLabSuppliesData(dataWithValues) {
    if (!dataWithValues || !dataWithValues.$values) {
        console.warn('Invalid data structure received');
        return [];
    }

    const labSupplies = dataWithValues.$values.reduce((acc, item) => {
        if (item.SupplyName) {
            acc.push(item);
        }

        // Handle nested supplier lab supplies
        if (item.Supplier && item.Supplier.LabSupplies && item.Supplier.LabSupplies.$values) {
            item.Supplier.LabSupplies.$values.forEach(supply => {
                if (supply.SupplyName) {
                    supply.Supplier = { SupplierName: item.Supplier.SupplierName };
                    acc.push(supply);
                }
            });
        }

        return acc;
    }, []);

    return labSupplies;
}

/**
 * Initializes the DataTable with lab supplies data
 * @param {Array} labSupplies - Array of lab supply objects
 */
function initializeDataTable(labSupplies) {
    dataTable = $('#tblData').DataTable({
        data: labSupplies,
        columns: [
            { 
                data: 'SupplyName', 
                width: "25%",
                title: "Supply Name"
            },
            { 
                data: 'QuantityOnHand', 
                width: "15%",
                title: "Quantity",
                render: function(data, type, row) {
                    // Highlight low stock items
                    if (row.QuantityOnHand <= row.ReorderPoint) {
                        return `<span class="badge bg-warning text-dark">${data}</span>`;
                    }
                    return data;
                }
            },
            { 
                data: 'ReorderPoint', 
                width: "15%",
                title: "Reorder Point"
            },
            { 
                data: 'Supplier.SupplierName', 
                width: "25%",
                title: "Supplier",
                defaultContent: 'N/A'
            },
            {
                data: 'SupplyID',
                width: "20%",
                title: "Actions",
                orderable: false,
                render: function (data) {
                    return `
                        <div class="w-75 btn-group" role="group">
                            <a href="/admin/LabSupplies/upsert?id=${data}" 
                               class="btn btn-primary mx-2" 
                               title="Edit Supply">
                                <i class="bi bi-pencil-square"></i> Edit
                            </a>
                            <button onclick="deleteSupply('/admin/LabSupplies/delete/${data}')" 
                                    class="btn btn-danger mx-2" 
                                    title="Delete Supply">
                                <i class="bi bi-trash-fill"></i> Delete
                            </button>
                        </div>`;
                }
            }
        ],
        order: [[0, 'asc']], // Sort by Supply Name by default
        pageLength: 10,
        language: {
            emptyTable: "No lab supplies found",
            search: "Search supplies:",
            lengthMenu: "Show _MENU_ supplies per page"
        },
        responsive: true
    });
}

/**
 * Deletes a lab supply after user confirmation
 * @async
 * @param {string} url - The API endpoint for deletion
 */
async function deleteSupply(url) {
    const result = await Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this action!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!",
        cancelButtonText: "Cancel"
    });

    if (result.isConfirmed) {
        try {
            const response = await fetch(url, {
                method: 'DELETE',
                headers: {
                    'Content-Type': 'application/json'
                }
            });

            if (!response.ok) {
                throw new Error('Delete operation failed');
            }

            const data = await response.json();

            if (data.success) {
                toastr.success(data.message || 'Supply deleted successfully');
                // Reload the page to refresh data
                setTimeout(() => location.reload(), 1000);
            } else {
                toastr.error(data.message || 'Failed to delete supply');
            }
        } catch (error) {
            console.error("Error deleting supply:", error);
            toastr.error('An error occurred while deleting the supply');
        }
    }
}

// Expose delete function globally for onclick handler
window.Delete = deleteSupply;

