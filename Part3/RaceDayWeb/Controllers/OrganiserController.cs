using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;
using static RaceDayWeb.Services.ApiService;

namespace RaceDayWeb.Controllers
{
    [Authorize(Roles = "Organiser")]
    public class OrganiserController : Controller
    {
        private readonly ApiService _apiService;

        public OrganiserController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // 1. DASHBOARD
        public async Task<IActionResult> Dashboard()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var allEvents = await _apiService.GetEventsAsync(token);
            return View(allEvents ?? new List<EventResponse>());
        }

        // 2. CREATE EVENT (GET - Shows the form)
        [HttpGet]
        public IActionResult CreateEvent()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> EditEvent(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var ev = await _apiService.GetEventByIdAsync(token, id);
            if (ev == null) return NotFound();

            // We pass the EventId in ViewBag so the form knows which event to update
            ViewBag.EventId = id;

            // Convert the response into the format the form expects
            var request = new EventRequest
            {
                EventId = id,
                OrganiserProfileId = ev.OrganiserProfileId,
                EventName = ev.EventName,
                Description = ev.Description,
                EventDate = ev.EventDate,
                Location = ev.Location,
                EventType = ev.EventType,
                DistanceKm = ev.DistanceKm
            };

            return View(request);
        }

        [HttpPost]
        public async Task<IActionResult> EditEvent(int id, EventRequest updatedEvent)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var success = await _apiService.UpdateEventAsync(token, id, updatedEvent);

            if (success)
            {
                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Failed to update the event.";
            ViewBag.EventId = id;
            return View(updatedEvent);
        }

        [HttpGet]
        public async Task<IActionResult> ManageCategories(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var categories = await _apiService.GetCategoriesAsync(token, id);

            ViewBag.EventId = id;
            // Pass the existing categories to the view, or an empty list if none
            return View(categories ?? new List<CategoryResponse>());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryRequest newCategory)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var success = await _apiService.CreateCategoryAsync(token, newCategory);

            // Refresh the page so the new category shows up
            return RedirectToAction("ManageCategories", new { id = newCategory.EventId });
        }

        [HttpGet]
        public async Task<IActionResult> ViewEnrolments(int eventId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            // 1. Get all enrolments for this organiser
            var allEnrolments = await _apiService.GetMyEventsEnrolmentsAsync(token);

            // 2. Get all categories for this specific event
            var eventCategories = await _apiService.GetCategoriesAsync(token, eventId);
            var categoryIds = eventCategories?.Select(c => c.CategoryId).ToList() ?? new List<int>();

            // 3. Filter the enrolments to only show ones that belong to this event's categories
            var eventEnrolments = allEnrolments?.Where(e => categoryIds.Contains(e.CategoryId)).ToList() ?? new List<EnrolmentResponse>();

            // 4. Pass the categories to the view so we can show their names instead of IDs
            ViewBag.Categories = eventCategories;
            ViewBag.EventId = eventId;

            return View(eventEnrolments);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            await _apiService.DeleteEventAsync(token, id);
            return RedirectToAction("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> ManageResults(int eventId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            // 1. Get all enrolments for this organiser's events
            var allEnrolments = await _apiService.GetMyEventsEnrolmentsAsync(token);

            // 2. Get all categories for this event
            var eventCategories = await _apiService.GetCategoriesAsync(token, eventId);
            var categoryIds = eventCategories?.Select(c => c.CategoryId).ToList() ?? new List<int>();

            // 3. Filter the enrolments for JUST this event
            var eventEnrolments = allEnrolments?.Where(e => categoryIds.Contains(e.CategoryId)).ToList() ?? new List<EnrolmentResponse>();

            // 4. Get existing results to see who already has a report card
            var allResults = await _apiService.GetResultsAsync(token);

            ViewBag.Categories = eventCategories;
            ViewBag.Results = allResults;
            ViewBag.EventId = eventId;

            return View(eventEnrolments);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitResult(ResultRequest newResult, int eventId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            await _apiService.CreateResultAsync(token, newResult);

            // Refresh the page to show the saved result
            return RedirectToAction("ManageResults", new { eventId = eventId });
        }

        [HttpPost]
        [RequestSizeLimit(104857600)]
        public async Task<IActionResult> CreateEvent(EventRequest newEvent, IFormFile imageFile)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            try
            {
                var profile = await _apiService.GetMyProfileAsync(token);
                if (profile?.OrganiserProfile == null) return BadRequest("Could not find Organiser profile.");

                newEvent.OrganiserProfileId = profile.OrganiserProfile.OrganiserProfileId;

                // This now returns an int? (the new ID) instead of a bool
                var newEventId = await _apiService.CreateEventAsync(token, newEvent);

                if (newEventId.HasValue)
                {
                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Use the exact ID to upload the image
                        await _apiService.UploadEventImageAsync(token, newEventId.Value, imageFile);
                    }
                    return RedirectToAction("Dashboard");
                }

                ViewBag.Error = "Failed to create the event.";
                return View(newEvent);
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Connection error: " + ex.Message;
                return View(newEvent);
            }
        }

    }

    }

