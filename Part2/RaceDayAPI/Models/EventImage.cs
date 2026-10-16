namespace RaceDayAPI.Models
{
    public class EventImage
    {
        public int EventImageId { get; set; }

        public int EventId { get; set; }

        public string ImageUrl { get; set; } = string.Empty;

        public string ImageName { get; set; } = string.Empty;

        // Navigation property
        public Event? Event { get; set; }
    }
}