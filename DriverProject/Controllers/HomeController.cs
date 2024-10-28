using DriverProject.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace DriverProject.Controllers
{
    public class HomeController : Controller
    {        

        private readonly UserManager<Employee> _userManager;
        private readonly SignInManager<Employee> _signInManager;

        //https://localhost:7158/Identity/Account/Login

        public HomeController(UserManager<Employee> userManager, SignInManager<Employee> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<IActionResult> Index()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                // If not logged in, redirect to login page
                return RedirectToPage("/Account/Login", new { area = "Identity" });
            }

            // Check if the user is an Admin
            var user = await _userManager.GetUserAsync(User);
            if (await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // Redirect to Employee index page for Admins
                return RedirectToAction("Index", "Employee");
            }
            else if (await _userManager.IsInRoleAsync(user, "Employee"))
            {
                // Redirect to Driver index page for Employees
                return RedirectToAction("Index", "Driver");
            }

            // Default redirect if no roles matched
            return RedirectToAction("Index", "Driver");
        }
    }
}
