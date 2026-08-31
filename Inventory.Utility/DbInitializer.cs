using Inventory.DataAccess.Data;
using Inventory.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Inventory.Utility
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AppDbContext _db;

        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            AppDbContext db)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _db = db;
        }

        public void Initialize()
        {
            // Apply pending migrations
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }
            catch (Exception ex)
            {
                // Migration already applied or database doesn't exist
                Console.WriteLine($"Migration error: {ex.Message}");
            }

            // Create roles if they don't exist
            try
            {
                if (!_roleManager.RoleExistsAsync(SD.Role_Admin).GetAwaiter().GetResult())
                {
                    Console.WriteLine("Creating roles...");
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Manager)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                    _roleManager.CreateAsync(new IdentityRole(SD.Role_Viewer)).GetAwaiter().GetResult();
                    Console.WriteLine("Roles created successfully!");

                    // Create default admin user
                    Console.WriteLine("Creating default admin user...");
                    var result = _userManager.CreateAsync(new ApplicationUser
                    {
                        UserName = "admin@inventory.com",
                        Email = "admin@inventory.com",
                        Name = "System Administrator",
                        PhoneNumber = "1234567890",
                        StreetAddress = "123 Admin St",
                        State = "IL",
                        PostalCode = "23456",
                        City = "Chicago",
                        EmailConfirmed = true
                    }, "Admin@123").GetAwaiter().GetResult();

                    if (result.Succeeded)
                    {
                        ApplicationUser? user = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "admin@inventory.com");
                        if (user != null)
                        {
                            _userManager.AddToRoleAsync(user, SD.Role_Admin).GetAwaiter().GetResult();
                            Console.WriteLine("Default admin user created successfully!");
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                    }
                }
                else
                {
                    Console.WriteLine("Roles already exist. Skipping initialization.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error initializing database: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }

            return;
        }
    }
}