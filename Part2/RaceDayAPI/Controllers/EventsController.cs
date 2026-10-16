
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Organiser")]
    public class EventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Events
        [AllowAnonymous]
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Event>>> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        // GET: api/Events/5
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Event>> GetEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return Ok(eventItem);
        }

        // POST: api/Events
        [HttpPost]
        public async Task<ActionResult<Event>> CreateEvent(Event eventItem)
        {
            _context.Events.Add(eventItem);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventId },
                eventItem);
        }

        // PUT: api/Events/2
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEvent(int id, Event eventItem)
        {
            if (id != eventItem.EventId)
            {
                return BadRequest();
            }

            var existingEvent = await _context.Events.FindAsync(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            existingEvent.OrganiserProfileId = eventItem.OrganiserProfileId;
            existingEvent.EventName = eventItem.EventName;
            existingEvent.Description = eventItem.Description;
            existingEvent.EventDate = eventItem.EventDate;
            existingEvent.Location = eventItem.Location;
            existingEvent.EventType = eventItem.EventType;
            existingEvent.DistanceKm = eventItem.DistanceKm;
            existingEvent.Status = eventItem.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Events/2
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            _context.Events.Remove(eventItem);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}

