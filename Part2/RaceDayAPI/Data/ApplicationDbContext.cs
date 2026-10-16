using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Models;

namespace RaceDayAPI.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(
            DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<OrganiserProfile> OrganiserProfiles { get; set; }
        public DbSet<ParticipantProfile> ParticipantProfiles { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Enrolment> Enrolments { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<RaceDayAPI.Models.Route> Routes { get; set; }
        public DbSet<EmergencyContact> EmergencyContacts { get; set; }
        public DbSet<Weather> WeatherRecords { get; set; }
        public DbSet<EventImage> EventImages { get; set; }
        public DbSet<Payment> Payments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure decimal precision
            modelBuilder.Entity<Category>()
                .Property(c => c.DistanceKm)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Category>()
                .Property(c => c.EntryFee)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Event>()
                .Property(e => e.DistanceKm)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Payment>()
                .Property(p => p.Amount)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Result>()
                .Property(r => r.AveragePace)
                .HasPrecision(10, 2);

            modelBuilder.Entity<RaceDayAPI.Models.Route>()
                .Property(r => r.DistanceKm)
                .HasPrecision(10, 2);

            modelBuilder.Entity<RaceDayAPI.Models.Route>()
                .Property(r => r.ElevationGain)
                .HasPrecision(10, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.Temperature)
                .HasPrecision(6, 2);

            modelBuilder.Entity<Weather>()
                .Property(w => w.WindSpeed)
                .HasPrecision(8, 2);

            // Each email address must be unique.
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // User and OrganiserProfile: one-to-one.
            modelBuilder.Entity<User>()
                .HasOne(u => u.OrganiserProfile)
                .WithOne(o => o.User)
                .HasForeignKey<OrganiserProfile>(o => o.UserId);

            // User and ParticipantProfile: one-to-one.
            modelBuilder.Entity<User>()
                .HasOne(u => u.ParticipantProfile)
                .WithOne(p => p.User)
                .HasForeignKey<ParticipantProfile>(p => p.UserId);

            // OrganiserProfile and Event: one-to-many.
            modelBuilder.Entity<Event>()
                .HasOne(e => e.OrganiserProfile)
                .WithMany(o => o.Events)
                .HasForeignKey(e => e.OrganiserProfileId);

            // Event and Category: one-to-many.
            modelBuilder.Entity<Category>()
                .HasOne(c => c.Event)
                .WithMany(e => e.Categories)
                .HasForeignKey(c => c.EventId);

            // Event and Route: one-to-many.
            modelBuilder.Entity<RaceDayAPI.Models.Route>()
                .HasOne(r => r.Event)
                .WithMany(e => e.Routes)
                .HasForeignKey(r => r.EventId);

            // Event and Weather: one-to-many.
            modelBuilder.Entity<Weather>()
                .HasOne(w => w.Event)
                .WithMany(e => e.WeatherRecords)
                .HasForeignKey(w => w.EventId);

            // Event and EventImage: one-to-many.
            modelBuilder.Entity<EventImage>()
                .HasOne(i => i.Event)
                .WithMany(e => e.Images)
                .HasForeignKey(i => i.EventId);

            // ParticipantProfile and Enrolment: one-to-many.
            modelBuilder.Entity<Enrolment>()
    .HasOne(e => e.ParticipantProfile)
    .WithMany(p => p.Enrolments)
    .HasForeignKey(e => e.ParticipantProfileId)
    .OnDelete(DeleteBehavior.NoAction);

            // Category and Enrolment: one-to-many.
            modelBuilder.Entity<Enrolment>()
    .HasOne(e => e.Category)
    .WithMany(c => c.Enrolments)
    .HasForeignKey(e => e.CategoryId)
    .OnDelete(DeleteBehavior.NoAction);

            // Enrolment and Result: one-to-one.
            modelBuilder.Entity<Enrolment>()
                .HasOne(e => e.Result)
                .WithOne(r => r.Enrolment)
                .HasForeignKey<Result>(r => r.EnrolmentId);

            // Enrolment and Payment: one-to-many.
            modelBuilder.Entity<Payment>()
                .HasOne(p => p.Enrolment)
                .WithMany(e => e.Payments)
                .HasForeignKey(p => p.EnrolmentId);

            // ParticipantProfile and EmergencyContact: one-to-many.
            modelBuilder.Entity<EmergencyContact>()
                .HasOne(c => c.ParticipantProfile)
                .WithMany(p => p.EmergencyContacts)
                .HasForeignKey(c => c.ParticipantProfileId);
        }
    }
}