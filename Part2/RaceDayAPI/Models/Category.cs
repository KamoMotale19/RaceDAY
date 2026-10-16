namespace RaceDayAPI.Models
{
    public class Category
    {
        public int CategoryId { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int EventId { get; set; }

        public decimal DistanceKm { get; set; }

        public decimal EntryFee { get; set; }

        public int MaxParticipants { get; set; }

        // Navigation properties
        public Event? Event { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();
    }
}