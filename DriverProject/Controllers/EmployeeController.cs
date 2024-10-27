
using DriverProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriverProject.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly UserManager<Employee> _userManager;
        

        public EmployeeController(UserManager<Employee> userManager)
        {
            _userManager = userManager;
            
        }

        // GET: EmployeeController
        public async Task<IActionResult> Index()
        {
            var employees = await _userManager.Users.ToListAsync();
            return View(employees);
        }

        // GET: EmployeeController/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _userManager.FindByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: EmployeeController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmployeeController/Create
        [HttpPost]        
        public async Task<IActionResult> Create(string email, string name, string password)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"ModelState Error: {error.ErrorMessage}");
                }
            }
            if (ModelState.IsValid)
            {
                var employee = new Employee
                {
                    UserName = email,
                    Email = email,
                    Name = name
                };

                var result = await _userManager.CreateAsync(employee, password);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        // GET: EmployeeController/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _userManager.FindByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: EmployeeController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, Employee model)
        {
            if (id != model.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var employee = await _userManager.FindByIdAsync(id);
                if (employee == null)
                {
                    return NotFound();
                }

                employee.Name = model.Name;
                employee.Email = model.Email;
                employee.UserName = model.Email;

                var result = await _userManager.UpdateAsync(employee);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(model);
        }

        // GET: EmployeeController/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _userManager.FindByIdAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // POST: EmployeeController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var employee = await _userManager.FindByIdAsync(id);
            if (employee != null)
            {
                var result = await _userManager.DeleteAsync(employee);
                if (result.Succeeded)
                {
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
