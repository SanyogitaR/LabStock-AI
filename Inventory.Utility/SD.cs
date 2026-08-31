using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inventory.Utility
{
    public static class SD
    {
        // Role definitions for organizational access control
        public const string Role_Admin = "Admin";
        public const string Role_Manager = "Manager";
        public const string Role_Employee = "Employee";
        public const string Role_Viewer = "Viewer";

        // Combined role strings for authorization
        public const string Role_Admin_Manager = "Admin,Manager";
        public const string Role_Admin_Manager_Employee = "Admin,Manager,Employee";
    }
}
