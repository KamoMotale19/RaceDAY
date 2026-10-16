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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ResultsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Results
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Result>>> GetResults()
        {
            var results = await _context.Results.ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/1
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/Results
        [HttpPost]
        public async Task<ActionResult<Result>> CreateResult(Result result)
        {
            var enrolmentExists = await _context.Enrolments
                .AnyAsync(e => e.EnrolmentId == result.EnrolmentId);

            if (!enrolmentExists)
            {
                return BadRequest(
                    "The specified EnrolmentId does not exist.");
            }

            var resultExists = await _context.Results
                .AnyAsync(r => r.EnrolmentId == result.EnrolmentId);

            if (resultExists)
            {
                return BadRequest(
                    "A result already exists for this enrolment.");
            }

            _context.Results.Add(result);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultId },
                result);
        }

        // PUT: api/Results/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateResult(
            int id, Result result)
        {
            if (id != result.ResultId)
            {
                return BadRequest();
            }

            var existingResult = await _context.Results.FindAsync(id);

            if (existingResult == null)
            {
                return NotFound();
            }

            existingResult.EnrolmentId = result.EnrolmentId;
            existingResult.FinishTime = result.FinishTime;
            existingResult.Position = result.Position;
            existingResult.AveragePace = result.AveragePace;
            existingResult.Status = result.Status;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Results/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            var result = await _context.Results.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            _context.Results.Remove(result);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}