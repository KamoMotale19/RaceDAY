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
    public class OrganiserProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrganiserProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/OrganiserProfiles
      
[HttpGet]
public async Task<ActionResult<IEnumerable<OrganiserProfile>>>
    GetOrganiserProfiles()
        {
            var profiles = await _context.OrganiserProfiles
                .Include(p => p.User)
                .Include(p => p.Events)
                .ToListAsync();

            return Ok(profiles);
        }



        // GET: api/OrganiserProfiles/1
       
        [HttpGet("{id}")]
public async Task<ActionResult<OrganiserProfile>>
    GetOrganiserProfile(int id)
        {
            var profile = await _context.OrganiserProfiles
                .Include(p => p.User)
                .Include(p => p.Events)
                .FirstOrDefaultAsync(p => p.OrganiserProfileId == id);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }



        // POST: api/OrganiserProfiles
        [HttpPost]
        public async Task<ActionResult<OrganiserProfile>>
            CreateOrganiserProfile(OrganiserProfile profile)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == profile.UserId);

            if (!userExists)
                return BadRequest("The specified UserId does not exist.");

            var profileExists = await _context.OrganiserProfiles
                .AnyAsync(p => p.UserId == profile.UserId);

            if (profileExists)
                return BadRequest(
                    "This user already has an organiser profile.");

            _context.OrganiserProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetOrganiserProfile),
                new { id = profile.OrganiserProfileId },
                profile);
        }

        // PUT: api/OrganiserProfiles/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateOrganiserProfile(
            int id, OrganiserProfile profile)
        {
            if (id != profile.OrganiserProfileId)
                return BadRequest();

            var existing = await _context.OrganiserProfiles.FindAsync(id);

            if (existing == null)
                return NotFound();

            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == profile.UserId);

            if (!userExists)
                return BadRequest("The specified UserId does not exist.");

            var duplicate = await _context.OrganiserProfiles
                .AnyAsync(p =>
                    p.UserId == profile.UserId &&
                    p.OrganiserProfileId != id);

            if (duplicate)
                return BadRequest(
                    "This user already has another organiser profile.");

            existing.UserId = profile.UserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/OrganiserProfiles/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganiserProfile(int id)
        {
            var profile =
                await _context.OrganiserProfiles.FindAsync(id);

            if (profile == null)
                return NotFound();

            var hasEvents = await _context.Events
                .AnyAsync(e => e.OrganiserProfileId == id);

            if (hasEvents)
                return BadRequest(
                    "Cannot delete an organiser profile linked to events.");

            _context.OrganiserProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}