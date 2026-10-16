using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace RaceDayAPI.Tests.Tests;

public class RoleTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public RoleTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLogin(string role)
    {
        var email = $"{role.ToLower()}_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            firstName = "Role",
            lastName = "Tester",
            email,
            password = "Password123!",
            role
        });

        var resp = await _client.PostAsJsonAsync("/api/Auth/login", new
        {
            email,
            password = "Password123!"
        });
        var json = await resp.Content.ReadAsStringAsync();
        var start = json.IndexOf("\"token\":\"") + 9;
        var end = json.IndexOf("\"", start);
        return json.Substring(start, end - start);
    }

    [Fact]
    public async Task Anonymous_CannotCreateEvent_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/Events", new
        {
            organiserProfileId = 1,
            eventName = "Anon Event",
            description = "test",
            eventDate = DateTime.UtcNow.AddMonths(1),
            location = "X",
            eventType = "Run",
            distanceKm = 5,
            status = "Upcoming"
        });
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CannotCreateEvent_Returns403()
    {
        var token = await RegisterAndLogin("Participant");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.PostAsJsonAsync("/api/Events", new
        {
            organiserProfileId = 1,
            eventName = "Part Event",
            description = "test",
            eventDate = DateTime.UtcNow.AddMonths(1),
            location = "X",
            eventType = "Run",
            distanceKm = 5,
            status = "Upcoming"
        });
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Organiser_CannotAccessEnrolmentsList_Returns403()
    {
        var token = await RegisterAndLogin("Organiser");
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Enrolments");
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task Anonymous_CanReadEvents_Returns200()
    {
        var response = await _client.GetAsync("/api/Events");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
