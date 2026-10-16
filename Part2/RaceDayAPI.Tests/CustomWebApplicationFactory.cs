using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Tests;

public class CustomWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly SqliteConnection _connection;

    public CustomWebApplicationFactory()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlite(_connection));

            using var scope = services.BuildServiceProvider().CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
            db.Database.EnsureCreated();

            if (!db.Users.Any())
                SeedTestData(db);
        });
    }

    private static void SeedTestData(ApplicationDbContext db)
    {
        var hasher = new PasswordHasher<User>();

        var organiser = new User
        {
            FirstName = "Test",
            LastName = "Organiser",
            Email = "organiser@test.com",
            Role = "Organiser"
        };
        organiser.PasswordHash = hasher.HashPassword(organiser, "Password123!");

        var participant = new User
        {
            FirstName = "Test",
            LastName = "Participant",
            Email = "participant@test.com",
            Role = "Participant"
        };
        participant.PasswordHash = hasher.HashPassword(participant, "Password123!");

        db.Users.AddRange(organiser, participant);
        db.SaveChanges();

        db.OrganiserProfiles.Add(new OrganiserProfile { UserId = organiser.UserId });
        db.ParticipantProfiles.Add(new ParticipantProfile { UserId = participant.UserId });
        db.SaveChanges();

        var evt = new Event
        {
            OrganiserProfileId = 1,
            EventName = "Test Marathon",
            Description = "Seeded event",
            EventDate = DateTime.UtcNow.AddMonths(3),
            Location = "Test City",
            EventType = "Run",
            DistanceKm = 42.2m,
            Status = "Upcoming"
        };
        db.Events.Add(evt);
        db.SaveChanges();

        db.Categories.Add(new Category
        {
            EventId = evt.EventId,
            CategoryName = "Open",
            DistanceKm = 42.2m,
            EntryFee = 100m,
            MaxParticipants = 100
        });
        db.SaveChanges();
    }
}
