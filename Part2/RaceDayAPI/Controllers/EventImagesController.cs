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
    public class EventImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EventImagesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EventImages
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EventImage>>> GetEventImages()
        {
            return Ok(await _context.EventImages.ToListAsync());
        }

        // GET: api/EventImages/1
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<EventImage>> GetEventImage(int id)
        {
            var image = await _context.EventImages.FindAsync(id);

            if (image == null)
                return NotFound();

            return Ok(image);
        }

        // POST: api/EventImages
        [HttpPost]
        public async Task<ActionResult<EventImage>>
            CreateEventImage(EventImage image)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == image.EventId);

            if (!eventExists)
                return BadRequest("The specified EventId does not exist.");

            _context.EventImages.Add(image);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventImage),
                new { id = image.EventImageId },
                image);
        }

        // PUT: api/EventImages/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEventImage(
            int id, EventImage image)
        {
            if (id != image.EventImageId)
                return BadRequest();

            var existing = await _context.EventImages.FindAsync(id);

            if (existing == null)
                return NotFound();

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == image.EventId);

            if (!eventExists)
                return BadRequest("The specified EventId does not exist.");

            existing.EventId = image.EventId;
            existing.ImageUrl = image.ImageUrl;
            existing.ImageName = image.ImageName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EventImages/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEventImage(int id)
        {
            var image = await _context.EventImages.FindAsync(id);

            if (image == null)
                return NotFound();

            _context.EventImages.Remove(image);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}