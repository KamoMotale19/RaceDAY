namespace RaceDayAPI.Models
{
    public class OrganiserProfile
    {
        public int OrganiserProfileId { get; set; }

        public int UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }

        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}