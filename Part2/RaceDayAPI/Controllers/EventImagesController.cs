using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;
using RaceDayAPI.Services;

namespace RaceDayAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventImagesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public EventImagesController(ApplicationDbContext context, BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        // GET: api/EventImages -> Publicly accessible!
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetEventImages()
        {
            var images = await _context.EventImages
                .Select(i => new
                {
                    i.EventImageId,
                    i.EventId,
                    i.ImageUrl,
                    i.ImageName
                })
                .ToListAsync();

            return Ok(images);
        }

        // POST: api/EventImages/upload/{eventId} -> Organiser only
        [Authorize(Roles = "Organiser")]
        [HttpPost("upload/{eventId}")]
        [Consumes("multipart/form-data")]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> UploadImage(int eventId, IFormFile file)
        {
            if (file == null || file.Length == 0) return BadRequest("No file provided.");

            try
            {
                var imageUrl = await _blobService.UploadFileAsync(file);
                var eventImage = new EventImage
                {
                    EventId = eventId,
                    ImageUrl = imageUrl,
                    ImageName = file.FileName
                };

                _context.EventImages.Add(eventImage);
                await _context.SaveChangesAsync();

                return Ok(new { imageUrl = imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Azure Storage Error: " + ex.Message);
            }
        }
    }
}