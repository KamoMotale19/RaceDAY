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
    public class WeatherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public WeatherController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Weather
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Weather>>> GetWeather()
        {
            return Ok(await _context.WeatherRecords.ToListAsync());
        }

        // GET: api/Weather/1
        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<Weather>> GetWeatherById(int id)
        {
            var weather = await _context.WeatherRecords.FindAsync(id);

            if (weather == null)
                return NotFound();

            return Ok(weather);
        }

        // POST: api/Weather
        [HttpPost]
        public async Task<ActionResult<Weather>> CreateWeather(Weather weather)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == weather.EventId);

            if (!eventExists)
                return BadRequest("The specified EventId does not exist.");

            _context.WeatherRecords.Add(weather);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetWeatherById),
                new { id = weather.WeatherId },
                weather);
        }

        // PUT: api/Weather/1
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeather(
            int id, Weather weather)
        {
            if (id != weather.WeatherId)
                return BadRequest();

            var existing = await _context.WeatherRecords.FindAsync(id);

            if (existing == null)
                return NotFound();

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventId == weather.EventId);

            if (!eventExists)
                return BadRequest("The specified EventId does not exist.");

            existing.EventId = weather.EventId;
            existing.RecordedAt = weather.RecordedAt;
            existing.Temperature = weather.Temperature;
            existing.Condition = weather.Condition;
            existing.WindSpeed = weather.WindSpeed;
            existing.Humidity = weather.Humidity;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Weather/1
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeather(int id)
        {
            var weather = await _context.WeatherRecords.FindAsync(id);

            if (weather == null)
                return NotFound();

            _context.WeatherRecords.Remove(weather);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}