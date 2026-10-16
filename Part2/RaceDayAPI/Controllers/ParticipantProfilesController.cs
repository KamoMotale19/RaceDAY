using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Participant")]
    public class ParticipantProfilesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ParticipantProfilesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ParticipantProfiles
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ParticipantProfile>>>
            GetParticipantProfiles()
        {
            return Ok(await _context.ParticipantProfiles.ToListAsync());
        }

        // GET: api/ParticipantProfiles/1
        
        [HttpGet("{id}")]
        public async Task<ActionResult<ParticipantProfile>>
            GetParticipantProfile(int id)
        {
            var profile =
                await _context.ParticipantProfiles.FindAsync(id);

            if (profile == null)
                return NotFound();

            return Ok(profile);
        }

        // POST: api/ParticipantProfiles
        [HttpPost]
        public async Task<ActionResult<ParticipantProfile>>
            CreateParticipantProfile(ParticipantProfile profile)
        {
            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == profile.UserId);

            if (!userExists)
                return BadRequest("The specified UserId does not exist.");

            var profileExists = await _context.ParticipantProfiles
                .AnyAsync(p => p.UserId == profile.UserId);

            if (profileExists)
                return BadRequest(
                    "This user already has a participant profile.");

            _context.ParticipantProfiles.Add(profile);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetParticipantProfile),
                new { id = profile.ParticipantProfileId },
                profile);
        }

        // PUT: api/ParticipantProfiles/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateParticipantProfile(
            int id, ParticipantProfile profile)
        {
            if (id != profile.ParticipantProfileId)
                return BadRequest();

            var existing =
                await _context.ParticipantProfiles.FindAsync(id);

            if (existing == null)
                return NotFound();

            var userExists = await _context.Users
                .AnyAsync(u => u.UserId == profile.UserId);

            if (!userExists)
                return BadRequest("The specified UserId does not exist.");

            var duplicate = await _context.ParticipantProfiles
                .AnyAsync(p =>
                    p.UserId == profile.UserId &&
                    p.ParticipantProfileId != id);

            if (duplicate)
                return BadRequest(
                    "This user already has another participant profile.");

            existing.UserId = profile.UserId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ParticipantProfiles/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteParticipantProfile(int id)
        {
            var profile =
                await _context.ParticipantProfiles.FindAsync(id);

            if (profile == null)
                return NotFound();

            var hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.ParticipantProfileId == id);

            var hasContacts = await _context.EmergencyContacts
                .AnyAsync(c => c.ParticipantProfileId == id);

            if (hasEnrolments || hasContacts)
                return BadRequest(
                    "Cannot delete a participant profile linked to enrolments or emergency contacts.");

            _context.ParticipantProfiles.Remove(profile);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}