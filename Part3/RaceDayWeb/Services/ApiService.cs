using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace RaceDayWeb.Services
{
    
    //  (The Service Class)
    
    public class ApiService
    {
        private readonly HttpClient _httpClient;

        public ApiService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoginResponse?> LoginAsync(string email, string password)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", new { email, password });
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<LoginResponse>();
            return null;
        }

        public async Task<List<EventResponse>?> GetEventsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Events");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<EventResponse>>();
            return null;
        }

        public async Task<MyProfileResponse?> GetMyProfileAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Users/me");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<MyProfileResponse>();
            return null;
        }

        public async Task<int?> CreateEventAsync(string token, EventRequest newEvent)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/Events", newEvent);

            if (response.IsSuccessStatusCode)
            {
                var createdEvent = await response.Content.ReadFromJsonAsync<EventResponse>();
                return createdEvent?.EventId;
            }
            return null;
        }

        public async Task<EventResponse?> GetEventByIdAsync(string token, int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"api/Events/{id}");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<EventResponse>();
            return null;
        }

        public async Task<bool> UpdateEventAsync(string token, int id, EventRequest updatedEvent)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync($"api/Events/{id}", updatedEvent);
            return response.IsSuccessStatusCode;
        }

        // Get all results (we will filter them in the controller)
        public async Task<List<ResultResponse>?> GetResultsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Results");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<ResultResponse>>();
            return null;
        }

        // Submit a new result
        public async Task<bool> CreateResultAsync(string token, ResultRequest newResult)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/Results", newResult);
            return response.IsSuccessStatusCode;
        }



        public async Task<bool> DeleteEventAsync(string token, int id)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"api/Events/{id}");
            return response.IsSuccessStatusCode;
        }
        // Get all categories for an event
        public async Task<List<CategoryResponse>?> GetCategoriesAsync(string token, int eventId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            // The API returns ALL categories, so we filter them here
            var response = await _httpClient.GetAsync("api/Categories");

            if (response.IsSuccessStatusCode)
            {
                var allCategories = await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
                return allCategories?.Where(c => c.EventId == eventId).ToList();
            }
            return null;
        }

        // Enrol in a category
        public async Task<bool> EnrolAsync(string token, int participantProfileId, int categoryId)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/Enrolments", new { participantProfileId, categoryId });
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> UploadEventImageAsync(string token, int eventId, IFormFile file)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // We use MultipartFormDataContent to send a file
            using var content = new MultipartFormDataContent();
            using var fileStream = file.OpenReadStream();
            var fileContent = new StreamContent(fileStream);

            // Tell the API what type of file it is (e.g., image/jpeg)
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
            content.Add(fileContent, "file", file.FileName);

            var response = await _httpClient.PostAsync($"api/EventImages/upload/{eventId}", content);
            return response.IsSuccessStatusCode;
        }


        public async Task<bool> UpdateProfileAsync(string token, string firstName, string lastName)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync("api/Users/me", new { firstName, lastName });
            return response.IsSuccessStatusCode;
        }

        // Get all enrolments for the logged-in participant
        public async Task<List<EnrolmentResponse>?> GetMyEnrolmentsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Enrolments");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<EnrolmentResponse>>();
            return null;
        }

        // Get results for the logged-in participant
        public async Task<List<ResultResponse>?> GetMyResultsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Results/my");
            if (response.IsSuccessStatusCode)
                return await response.Content.ReadFromJsonAsync<List<ResultResponse>>();
            return null;
        }

        public async Task<List<CategoryResponse>?> GetAllCategoriesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Categories");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<CategoryResponse>>();
            return null;
        }

        // Create a new category
        public async Task<bool> CreateCategoryAsync(string token, CategoryRequest newCategory)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PostAsJsonAsync("api/Categories", newCategory);
            return response.IsSuccessStatusCode;
        }

        public async Task<List<EventImageResponse>?> GetEventImagesAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/EventImages");
            if (response.IsSuccessStatusCode) return await response.Content.ReadFromJsonAsync<List<EventImageResponse>>();
            return null;
        }

        // Get all enrolments for the organiser's events
        public async Task<List<EnrolmentResponse>?> GetMyEventsEnrolmentsAsync(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("api/Enrolments/my-events");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<EnrolmentResponse>>();
            }
            return null;
        }

    }




    // (Data Models)
   
    public class LoginResponse
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }



    public class ResultResponse
    {
        public int ResultId { get; set; }
        public int EnrolmentId { get; set; }
        public string FinishTime { get; set; } = string.Empty;
        public int Position { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ResultRequest
    {
        public int EnrolmentId { get; set; }
        public string FinishTime { get; set; } = string.Empty; 
        public int Position { get; set; }
        public decimal AveragePace { get; set; } = 5.0m;
        public string Status { get; set; } = "Finished";
    }
    public class EventImageResponse
    {
        public int EventImageId { get; set; }
        public int EventId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string ImageName { get; set; } = string.Empty;
    }


    public class EventResponse
    {
        public int EventId { get; set; }
        public int OrganiserProfileId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class EnrolmentResponse
    {
        public int EnrolmentId { get; set; }
        public int ParticipantProfileId { get; set; }
        public int CategoryId { get; set; }
        public string Status { get; set; } = "Pending";
    }

    public class EventRequest
    {
        public int EventId { get; set; }
        public int OrganiserProfileId { get; set; }
        public string EventName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public string EventType { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public string Status { get; set; } = "Upcoming";
    }

    public class MyProfileResponse
    {
        public UserInfo User { get; set; } = new();
        public OrganiserProfileInfo? OrganiserProfile { get; set; }

        public ParticipantProfileInfo? ParticipantProfile { get; set; }
    }

    public class UserInfo
    {
        public int UserId { get; set; }
        public string FirstName { get; set; } = string.Empty; // 
        public string LastName { get; set; } = string.Empty;  
        public string Email { get; set; } = string.Empty;
    }

    public class OrganiserProfileInfo
    {
        public int OrganiserProfileId { get; set; }
        public int UserId { get; set; }
    }

    public class ParticipantProfileInfo
    {
        public int ParticipantProfileId { get; set; }
        public int UserId { get; set; }
    }

    public class CategoryResponse
    {
        public int CategoryId { get; set; }
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
    }

    public class CategoryRequest
    {
        public int EventId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public decimal DistanceKm { get; set; }
        public decimal EntryFee { get; set; }
        public int MaxParticipants { get; set; }
    }
}