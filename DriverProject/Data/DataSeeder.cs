using DriverProject.Models;
using Microsoft.AspNetCore.Identity;

namespace DriverProject.Data
{
    public class DataSeeder
    {
        private readonly UserManager<Employee> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DataSeeder(UserManager<Employee> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task SeedAdminAsync()
        {
            // Ensure the "Admin" role exists
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Seed the first admin
            await CreateAdminUser("admin@example.com", "Admin123!");

            //// Seed a second admin with a different email
            //await CreateAdminUser("admin2@example.com", "Admin456!");
        }

        // Helper method to create an admin user
        private async Task CreateAdminUser(string email, string password)
        {
            var adminUser = await _userManager.FindByEmailAsync(email);
            if (adminUser == null)
            {
                var newAdmin = new Employee
                {
                    UserName = email,
                    Email = email,
                    Name = "System Administrator",
                    EmailConfirmed = true
                };

                var result = await _userManager.CreateAsync(newAdmin, password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(newAdmin, "Admin");
                    Console.WriteLine($"Admin user {email} created successfully.");
                }
                else
                {
                    Console.WriteLine($"Failed to create admin user {email}: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
            else
            {
                Console.WriteLine($"Admin user {email} already exists.");
            }
        }


    }
}
