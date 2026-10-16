
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
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories.ToListAsync();

            return Ok(categories);
        }

        // GET: api/Categories/1
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }

        // POST: api/Categories
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(
            Category category)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == category.EventId);

            if (!eventExists)
            {
                return BadRequest(
                    "The specified EventId does not exist.");
            }

            _context.Categories.Add(category);

            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryId },
                category);
        }

        // PUT: api/Categories/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateCategory(
            int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            var existingCategory = await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == category.EventId);

            if (!eventExists)
            {
                return BadRequest(
                    "The specified EventId does not exist.");
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.EventId = category.EventId;
            existingCategory.DistanceKm = category.DistanceKm;
            existingCategory.EntryFee = category.EntryFee;
            existingCategory.MaxParticipants = category.MaxParticipants;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categories/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
