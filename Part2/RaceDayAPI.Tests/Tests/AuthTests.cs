using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace RaceDayAPI.Tests.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_NewParticipant_Returns200()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            firstName = "Alice",
            lastName = "Tester",
            email = $"alice_{Guid.NewGuid()}@test.com",
            password = "Password123!",
            role = "Participant"
        });
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Register_InvalidRole_Returns400()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            firstName = "Bad",
            lastName = "Role",
            email = $"bad_{Guid.NewGuid()}@test.com",
            password = "Password123!",
            role = "Admin"
        });
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Register_DuplicateEmail_Returns400()
    {
        var email = $"dupe_{Guid.NewGuid()}@test.com";
        var body = new
        {
            firstName = "Dupe",
            lastName = "User",
            email,
            password = "Password123!",
            role = "Participant"
        };
        var first = await _client.PostAsJsonAsync("/api/Auth/register", body);
        Assert.Equal(HttpStatusCode.OK, first.StatusCode);

        var second = await _client.PostAsJsonAsync("/api/Auth/register", body);
        Assert.Equal(HttpStatusCode.BadRequest, second.StatusCode);
    }

    [Fact]
    public async Task Login_ValidCredentials_ReturnsToken()
    {
        var email = $"login_{Guid.NewGuid()}@test.com";
        await _client.PostAsJsonAsync("/api/Auth/register", new
        {
            firstName = "Login",
            lastName = "Test",
            email,
            password = "Password123!",
            role = "Participant"
        });

        var response = await _client.PostAsJsonAsync("/api/Auth/login", new
        {
            email,
            password = "Password123!"
        });

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("token", json);
    }

    [Fact]
    public async Task Login_WrongPassword_Returns401()
    {
        var response = await _client.PostAsJsonAsync("/api/Auth/login", new
        {
            email = "participant@test.com",
            password = "WrongPassword!"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}
