using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RaceDayWeb.Services;

namespace RaceDayWeb.Controllers
{
    [Authorize(Roles = "Participant")]
    public class ParticipantController : Controller
    {
        private readonly ApiService _apiService;

        public ParticipantController(ApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<IActionResult> BrowseEvents()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var events = await _apiService.GetEventsAsync(token);
            return View(events ?? new List<EventResponse>());
        }

        
        [HttpGet]
        public async Task<IActionResult> EventDetails(int id)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var ev = await _apiService.GetEventByIdAsync(token, id);
            if (ev == null) return NotFound();

            var categories = await _apiService.GetCategoriesAsync(token, id);
            ViewBag.Categories = categories;

            // --- DIAGNOSTIC CODE ---
            var allImages = await _apiService.GetEventImagesAsync(token);

            if (allImages == null)
            {
               
                ViewBag.BannerImageUrl = "API_RETURNED_NULL (Check if API is running on port 7295)";
            }
            else
            {
                var eventImage = allImages.FirstOrDefault(i => i.EventId == id);
                if (eventImage != null)
                {
                    ViewBag.BannerImageUrl = eventImage.ImageUrl;
                }
                else
                {
                    
                    ViewBag.BannerImageUrl = $"NO_IMAGE_MATCHES_EVENT_ID_{id} (API returned {allImages.Count} total images)";
                }
            }
            

            return View(ev);
        }

        [HttpPost]
        public async Task<IActionResult> Enrol(int categoryId, int eventId)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var profile = await _apiService.GetMyProfileAsync(token);
            if (profile?.ParticipantProfile == null) return BadRequest("Profile not found");

            await _apiService.EnrolAsync(token, profile.ParticipantProfile.ParticipantProfileId, categoryId);

            return RedirectToAction("MyEvents");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var profile = await _apiService.GetMyProfileAsync(token);
            if (profile == null) return NotFound();

            return View(profile);
        }

        [HttpPost]
        public async Task<IActionResult> Profile(string firstName, string lastName)
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var success = await _apiService.UpdateProfileAsync(token, firstName, lastName);

            if (success)
            {
                ViewBag.Success = "Profile updated successfully!";
            }
            else
            {
                ViewBag.Error = "Failed to update profile.";
            }

            var profile = await _apiService.GetMyProfileAsync(token);
            return View(profile);
        }

        public async Task<IActionResult> MyEvents()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            var profile = await _apiService.GetMyProfileAsync(token);
            var allEnrolments = await _apiService.GetMyEnrolmentsAsync(token);

            var myEnrolments = allEnrolments?.Where(e => e.ParticipantProfileId == profile?.ParticipantProfile?.ParticipantProfileId).ToList();

            ViewBag.Events = await _apiService.GetEventsAsync(token);
            ViewBag.Categories = await _apiService.GetAllCategoriesAsync(token);

            return View(myEnrolments ?? new List<EnrolmentResponse>());
        }
        [HttpGet]
        

        public async Task<IActionResult> MyResults()
        {
            var token = HttpContext.Session.GetString("JwtToken");
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Login", "Account");

            // 1. Get the user's profile and their enrolments
            var profile = await _apiService.GetMyProfileAsync(token);
            var myEnrolments = await _apiService.GetMyEnrolmentsAsync(token);

            // 2. Filter enrolments to only this user
            var participantEnrolments = myEnrolments?.Where(e => e.ParticipantProfileId == profile?.ParticipantProfile?.ParticipantProfileId).ToList() ?? new List<EnrolmentResponse>();
            var enrolmentIds = participantEnrolments.Select(e => e.EnrolmentId).ToList();

            // 3. Get ALL results and filter them in memory based on the user's enrolment IDs
            var allResults = await _apiService.GetResultsAsync(token);
            var myResults = allResults?.Where(r => enrolmentIds.Contains(r.EnrolmentId)).ToList() ?? new List<ResultResponse>();

            // 4. Pass data to the view
            ViewBag.Enrolments = myEnrolments;
            ViewBag.Events = await _apiService.GetEventsAsync(token);
            ViewBag.Categories = await _apiService.GetAllCategoriesAsync(token);
            ViewBag.EventImages = await _apiService.GetEventImagesAsync(token);

            return View(myResults);
        }
    }
}