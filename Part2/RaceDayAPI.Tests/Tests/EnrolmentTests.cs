using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace RaceDayAPI.Tests.Tests;

public class EnrolmentTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public EnrolmentTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterParticipantAndGetToken()
    {
        var email = $"part_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            firstName = "Part",
            lastName = "Tester",
            email,
            password = "Password123!",
            role = "Participant"
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
    public async Task Participant_CanEnrol_Returns201()
    {
        var token = await RegisterParticipantAndGetToken();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var meResp = await _client.GetAsync("/api/Users/me");
        var meJson = await meResp.Content.ReadAsStringAsync();
        var idx = meJson.IndexOf("participantProfileId\":") + 22;
        var end = meJson.IndexOf(",", idx);
        var partProfileId = int.Parse(meJson.Substring(idx, end - idx));

        var response = await _client.PostAsJsonAsync("/api/Enrolments", new
        {
            participantProfileId = partProfileId,
            categoryId = 1
        });
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task Participant_CanViewOwnEnrolments_Returns200()
    {
        var token = await RegisterParticipantAndGetToken();
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/Enrolments");
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
