using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using RaceDayRoute = RaceDayAPI.Models.Route;
using Microsoft.AspNetCore.Authorization;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Organiser")]

    public class RoutesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RoutesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Routes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RaceDayRoute>>> GetRoutes()
        {
            return Ok(await _context.Routes.ToListAsync());
        }

        // GET: api/Routes/1
        [HttpGet("{id}")]
        public async Task<ActionResult<RaceDayRoute>> GetRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);

            if (route == null)
                return NotFound();

            return Ok(route);
        }

        // POST: api/Routes
        [HttpPost]
        public async Task<ActionResult<RaceDayRoute>> CreateRoute(RaceDayRoute route)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == route.EventId);

            if (!eventExists)
                return BadRequest("The specified EventId does not exist.");

            _context.Routes.Add(route);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetRoute),
                new { id = route.RouteId },
                route);
        }

        // PUT: api/Routes/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoute(
            int id, RaceDayRoute route)
        {
            if (id != route.RouteId)
                return BadRequest();

            var existing = await _context.Routes.FindAsync(id);

            if (existing == null)
                return NotFound();

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == route.EventId);

            if (!eventExists)
                return BadRequest("The specified EventId does not exist.");

            existing.EventId = route.EventId;
            existing.DistanceKm = route.DistanceKm;
            existing.RouteName = route.RouteName;
            existing.StartLocation = route.StartLocation;
            existing.FinishLocation = route.FinishLocation;
            existing.RouteUrl = route.RouteUrl;
            existing.ElevationGain = route.ElevationGain;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Routes/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoute(int id)
        {
            var route = await _context.Routes.FindAsync(id);

            if (route == null)
                return NotFound();

            _context.Routes.Remove(route);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}