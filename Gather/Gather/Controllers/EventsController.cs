using Microsoft.AspNetCore.Mvc;
using Gather.Models;
using Microsoft.Extensions.Logging;
using Gather.Data;
using Microsoft.EntityFrameworkCore;

namespace Gather.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EventController : ControllerBase
    {

        private readonly GatherDbContext _context;

        // GET: api/event/public
        [HttpGet("public")]
        public async Task<IActionResult> GetPublicEvents([FromQuery] double userLat, [FromQuery] double userLng, [FromQuery] double radiusInKm = 10)
        {
            var events = await _context.Events
                .FromSqlRaw(@"
                    SELECT * FROM Events
                    WHERE IsPrivate = 0 AND (
                        6371 * acos(
                            cos(radians({0})) * cos(radians(Latitude)) *
                            cos(radians(Longitude) - radians({1})) +
                            sin(radians({0})) * sin(radians(Latitude))
                           )
                    ) < {2}", userLat, userLng, radiusInKm)
                .ToListAsync();

            return Ok(events);
        }


        [HttpGet("nearby")]
        public async Task<IActionResult> GetNearbyEvents([FromQuery] double lat, [FromQuery] double lng, [FromQuery] double radiusInKm = 10)
        {
            var events = await _context.Events
                .FromSqlRaw(@"
            SELECT * FROM Events
            WHERE (
                6371 * acos(
                    cos(radians({0})) * cos(radians(Latitude)) *
                    cos(radians(Longitude) - radians({1})) +
                    sin(radians({0})) * sin(radians(Latitude))
                )
            ) < {2}", lat, lng, radiusInKm)
                .ToListAsync();
            if (events.Count != 0)
            {
                var result = events.Select(e => new
                {
                    e.Id,
                    e.Title,
                    e.Description,
                    e.EventDate,
                    e.IsPublic,
                    Latitude = e.IsPublic ? (double?)null : e.Latitude,
                    Longitude = e.IsPublic ? (double?)null : e.Longitude
                });

                return Ok(result);
            }
            return NotFound("Inga event hittades");
            
        }


        // GET: api/event/myEvents
        [HttpGet("myEvents")]
        public IActionResult GetMyEvents([FromQuery] Guid userId)
        {
            // Returnera events som skapat av användaren
            var myEvents = _context.Events.Where(e => e.HostUserId == userId).ToList();
            if (myEvents.Count != 0)
            {
                return Ok(myEvents);
            }
            return NotFound("Du har inte skapat några event än");
        }

        // POST: api/event
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Events model)
        {
            // Skapa nytt event

            if (string.IsNullOrEmpty(model.Title) || model.EventDate == null)
            {
                return BadRequest("Vänligen fyll i alla obligatoriska fält.");
            }
            
             _context.Events.Add(model);
             await _context.SaveChangesAsync();

             return CreatedAtAction(nameof(GetEventById), new { id = model.Id }, model);           
        }

        // GET: api/event/{id}
        [HttpGet("{id}")]
        public IActionResult GetEventById(Guid id)
        {
            // Returnera specifikt event

            var Event = _context.Events.FirstOrDefault(e => e.Id == id);            
            return Ok(Event);            
        }

        // POST: api/event/{id}/apply
        [HttpPost("{id}/apply")]
        public async Task<IActionResult> ApplyToPrivateEvent(Guid id, [FromQuery] Guid userId)
        {
            // Logik för att ansöka till ett privat event
            
            var targetEvent = await _context.Events.FirstOrDefaultAsync(e => e.Id == id);
            if (targetEvent == null)
            {
                return NotFound("Eventet hittades inte");
            }
            var existingApplication = await _context.Applications.FirstOrDefaultAsync(e => e.EventId == id && e.UserId == userId);
            if (existingApplication != null)
            {
                return BadRequest("Du har redan ansökt till detta event, invänta värdens svar!");
            }

            var application = new Applications
            {
                EventId = id,
                UserId = userId
            };
            
            _context.Applications.Add(application);

            await _context.SaveChangesAsync();

            return Ok("Din ansökan är nu skickad till eventets värd");
        }

        // POST: api/event/{id}/approve/{userId}
        [HttpPost("{id}/approve/{userId}")]
        public async Task<IActionResult> ApproveParticipant(Guid id, Guid userId)
        {
            // Värden godkänner en deltagare

            var application = await _context.Applications.FirstOrDefaultAsync(a => a.EventId == id && a.UserId == userId);

            if (application == null)
            {
                return NotFound("Ansökan hittades inte");
            }

            application.Status = true;
            await _context.SaveChangesAsync();

            return Ok("Gästen är nu godkänd!");
        }

        // DELETE: api/event/{id}
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(Guid id)
        {
            // Ta bort event (av värd eller admin)

            var EventToDelete = _context.Events.FirstOrDefault(e => e.Id == id);
            if (EventToDelete == null)
            {
                return NotFound("Vi kunde inte hitta ditt event, försök igen!");
            }
            var applicationsToDelete = _context.Applications.Where(a => a.EventId == id);
            _context.Applications.RemoveRange(applicationsToDelete);

            _context.Events.Remove(EventToDelete);

            _context.SaveChanges();
            
            return NoContent();
        }
    }
}
