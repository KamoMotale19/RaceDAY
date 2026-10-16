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
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EnrolmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Enrolments

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEnrolments()
        {
            var enrolments = await _context.Enrolments.ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/Enrolments/1
        [HttpGet("{id}")]
        public async Task<ActionResult<Enrolment>> GetEnrolment(int id)
        {
            var enrolment = await _context.Enrolments.FindAsync(id);

            if (enrolment == null)
            {
                return NotFound();
            }

            return Ok(enrolment);
        }

        // POST: api/Enrolments
        [HttpPost]
        public async Task<ActionResult<Enrolment>> CreateEnrolment(
            Enrolment enrolment)
        {
            var participantExists = await _context.ParticipantProfiles
                .AnyAsync(p =>
                    p.ParticipantProfileId == enrolment.ParticipantProfileId);

            if (!participantExists)
            {
                return BadRequest(
                    "The specified ParticipantProfileId does not exist.");
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == enrolment.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(
                    "The specified CategoryId does not exist.");
            }

            _context.Enrolments.Add(enrolment);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = enrolment.EnrolmentId },
                enrolment);
        }

        // PUT: api/Enrolments/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEnrolment(
            int id, Enrolment enrolment)
        {
            if (id != enrolment.EnrolmentId)
            {
                return BadRequest();
            }

            var existingEnrolment =
                await _context.Enrolments.FindAsync(id);

            if (existingEnrolment == null)
            {
                return NotFound();
            }

            var participantExists = await _context.ParticipantProfiles
                .AnyAsync(p =>
                    p.ParticipantProfileId == enrolment.ParticipantProfileId);

            if (!participantExists)
            {
                return BadRequest(
                    "The specified ParticipantProfileId does not exist.");
            }

            var categoryExists = await _context.Categories
                .AnyAsync(c => c.CategoryId == enrolment.CategoryId);

            if (!categoryExists)
            {
                return BadRequest(
                    "The specified CategoryId does not exist.");
            }

            existingEnrolment.ParticipantProfileId =
                enrolment.ParticipantProfileId;

            existingEnrolment.CategoryId = enrolment.CategoryId;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Enrolments/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var enrolment = await _context.Enrolments
                .Include(e => e.Payments)
                .Include(e => e.Result)
                .FirstOrDefaultAsync(e => e.EnrolmentId == id);

            if (enrolment == null)
            {
                return NotFound();
            }

            if (enrolment.Payments.Any() || enrolment.Result != null)
            {
                return BadRequest(
                    "Cannot delete this enrolment because it has associated payments or a result.");
            }

            _context.Enrolments.Remove(enrolment);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}