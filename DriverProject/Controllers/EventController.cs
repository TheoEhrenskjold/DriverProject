using DriverProject.Data;
using DriverProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DriverProject.Controllers
{
    public class EventController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public EventController(ApplicationDbContext context)
        {
            _context = context;
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

        public async Task<IActionResult> Index(string searchString)
        {
            var drivers = from d in _context.Drivers select d;

            if (!String.IsNullOrEmpty(searchString))
            {
                drivers = drivers.Where(s => s.DriverName.Contains(searchString));
            }
            return View(await drivers.ToListAsync());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEvent(Guid id, Event newEvent)
        {
            var driver = await _context.Drivers.FindAsync(id);
            if (driver == null)
            {
                return NotFound();
            }

            newEvent.DriverID = driver.DriverID;
            _context.Events.Add(newEvent);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = driver.DriverID });
        }

        public async Task<IActionResult> FilterByDate(Guid driverId, DateTime startDate, DateTime endDate)
        {
            var events = await _context.Events
                                       .Where(e => e.DriverID == driverId && e.NoteDate >= startDate && e.NoteDate <= endDate)
                                       .ToListAsync();
            return View(events);
        }

        public async Task<IActionResult> GetRecentEvents()
        {
            var recentEvents = await _context.Events
                                             .Where(e => e.NoteDate >= DateTime.Now.AddHours(-12))
                                             .ToListAsync();
            return View(recentEvents);
        }

        public async Task<IActionResult> RecentNotifications(int hours = 12)
        {
            var since = DateTime.Now.AddHours(-hours);
            var recentEvents = await _context.Events
                .Where(e => e.NoteDate >= since)
                .ToListAsync();

            return View(recentEvents); // Visa i tabellform i vyn
        }
    }
}
