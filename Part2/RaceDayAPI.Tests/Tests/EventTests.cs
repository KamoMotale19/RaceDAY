using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace RaceDayAPI.Tests.Tests;

public class EventTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EventTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterOrganiserAndGetToken()
    {
        var email = $"org_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            firstName = "Org",
            lastName = "Tester",
            email,
            password = "Password123!",
            role = "Organiser"
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
    public async Task Organiser_CanCreateEvent_Returns201()
    {
        var token = await RegisterOrganiserAndGetToken();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var meResp = await _client.GetAsync("/api/Users/me");
        var meJson = await meResp.Content.ReadAsStringAsync();
        var idx = meJson.IndexOf("organiserProfileId\":") + 20;
        var end = meJson.IndexOf(",", idx);
        var orgProfileId = int.Parse(meJson.Substring(idx, end - idx));

        var response = await _client.PostAsJsonAsync("/api/Events", new
        {
            organiserProfileId = orgProfileId,
            eventName = "New Test Event",
            description = "created in test",
            eventDate = DateTime.UtcNow.AddMonths(2),
            location = "Testville",
            eventType = "Run",
            distanceKm = 10,
            status = "Upcoming"
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task GetEvents_ReturnsOk()
    {
        var response = await _client.GetAsync("/api/Events");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
