using DriverProject.Data;
using DriverProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriverProject.Controllers
{
    public class DriverController : Controller
    {
        private readonly ApplicationDbContext _context;

        public DriverController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchName)
        {
            var drivers = _context.Drivers.AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                drivers = drivers.Where(d => d.DriverName.Contains(searchName));  // Ensure Name or DriverName is mapped correctly
            }

            var driversList = await drivers.ToListAsync();
            return View(driversList);  // Pass the full Driver objects to the view
        }

        // Visa specifik förare med historik
        public async Task<IActionResult> Details(Guid id)
        {
            var driver = await _context.Drivers
                .Include(d => d.Events)
                .FirstOrDefaultAsync(m => m.DriverID == id);

            if (driver == null)
            {
                return NotFound();
            }

            return View(driver);
        }

        // Skapa ny förare
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DriverName,CarReg")] Driver model)
        {
            if (!ModelState.IsValid)
            {
                // Log each error in ModelState to understand why it's invalid
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"Key: {key}, Error: {error.ErrorMessage}");
                    }
                }
                return View(model);
            }

            model.DriverID = Guid.NewGuid();  // Set a new ID for the driver

            _context.Drivers.Add(model);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }



        // Redigera förare
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, Driver driver)
        {
            if (id != driver.DriverID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                _context.Update(driver);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(driver);
        }

        // Ta bort förare
        [HttpGet]
        public async Task<IActionResult> Delete(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var driver = await _context.Drivers.FindAsync(id);
            _context.Drivers.Remove(driver);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


    }
}
