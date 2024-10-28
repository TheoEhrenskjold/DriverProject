using DriverProject.Data;
using DriverProject.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriverProject.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Employee> _userManager;

        public EventController(ApplicationDbContext context, UserManager<Employee> userManager)
        {
            _context = context;
            _userManager = userManager;
        }
        public async Task<IActionResult> Details(Guid id)
        {
            var driver = await _context.Drivers
                                       .Include(d => d.Events) // Include related events
                                       .FirstOrDefaultAsync(m => m.DriverID == id);
            if (driver == null)
            {
                return NotFound();
            }
            return View(driver);
        }

        public async Task<IActionResult> Index(Guid driverId, DateTime? startDate, DateTime? endDate)
        {
            var driver = await _context.Drivers
                .Include(d => d.Events)
                .FirstOrDefaultAsync(d => d.DriverID == driverId);

            if (driver == null)
            {
                return NotFound("Driver not found.");
            }

            // Use ViewData to pass DriverID and DriverName to the view
            ViewData["DriverID"] = driver.DriverID;
            ViewData["DriverName"] = driver.DriverName;

            // Filter events by date range if necessary
            var events = driver.Events.AsQueryable();
            if (startDate.HasValue)
            {
                events = events.Where(e => e.NoteDate >= startDate.Value);
            }
            if (endDate.HasValue)
            {
                events = events.Where(e => e.NoteDate <= endDate.Value);
            }

            return View(events.ToList()); // Return the filtered list of events
        }

        [HttpGet]
        public IActionResult Create(Guid driverId)
        {
            if (driverId == Guid.Empty)
            {
                return NotFound("Driver ID is required to create an event.");
            }

            ViewData["DriverID"] = driverId; // Pass driverId to the view
            return View();
        }

        // POST: Driver/CreateEvent
        [HttpPost]
        public async Task<IActionResult> Create(Event model)
        {
            if (ModelState.IsValid)
            {
                _context.Events.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", new { driverId = model.DriverID });
            }

            return View(model);
        }

        public async Task<IActionResult> FilterByDate(Guid driverId, DateTime startDate, DateTime endDate)
        {
            var events = await _context.Events
                                       .Where(e => e.DriverID == driverId && e.NoteDate >= startDate && e.NoteDate <= endDate)
                                       .ToListAsync();
            return View(events);
        }        


        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }

        // POST: Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);
            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction("Index", new { driverId = eventItem.DriverID });
        }
    }
}
