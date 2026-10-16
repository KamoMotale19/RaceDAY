using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmergencyContactsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EmergencyContactsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EmergencyContacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmergencyContact>>>
            GetEmergencyContacts()
        {
            return Ok(await _context.EmergencyContacts.ToListAsync());
        }

        // GET: api/EmergencyContacts/1
        [HttpGet("{id}")]
        public async Task<ActionResult<EmergencyContact>>
            GetEmergencyContact(int id)
        {
            var contact =
                await _context.EmergencyContacts.FindAsync(id);

            if (contact == null)
                return NotFound();

            return Ok(contact);
        }

        // POST: api/EmergencyContacts
        [HttpPost]
        public async Task<ActionResult<EmergencyContact>>
            CreateEmergencyContact(EmergencyContact contact)
        {
            var participantExists = await _context.ParticipantProfiles
                .AnyAsync(p =>
                    p.ParticipantProfileId == contact.ParticipantProfileId);

            if (!participantExists)
                return BadRequest(
                    "The specified ParticipantProfileId does not exist.");

            _context.EmergencyContacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEmergencyContact),
                new { id = contact.EmergencyContactId },
                contact);
        }

        // PUT: api/EmergencyContacts/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmergencyContact(
            int id, EmergencyContact contact)
        {
            if (id != contact.EmergencyContactId)
                return BadRequest();

            var existing =
                await _context.EmergencyContacts.FindAsync(id);

            if (existing == null)
                return NotFound();

            var participantExists = await _context.ParticipantProfiles
                .AnyAsync(p =>
                    p.ParticipantProfileId == contact.ParticipantProfileId);

            if (!participantExists)
                return BadRequest(
                    "The specified ParticipantProfileId does not exist.");

            existing.ParticipantProfileId = contact.ParticipantProfileId;
            existing.ContactName = contact.ContactName;
            existing.Relationship = contact.Relationship;
            existing.PhoneNumber = contact.PhoneNumber;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EmergencyContacts/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmergencyContact(int id)
        {
            var contact =
                await _context.EmergencyContacts.FindAsync(id);

            if (contact == null)
                return NotFound();

            _context.EmergencyContacts.Remove(contact);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}