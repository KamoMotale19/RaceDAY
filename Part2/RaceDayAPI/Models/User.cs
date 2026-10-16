namespace RaceDayAPI.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = string.Empty;

        // Navigation properties
        public OrganiserProfile? OrganiserProfile { get; set; }

        public ParticipantProfile? ParticipantProfile { get; set; }
    }
}