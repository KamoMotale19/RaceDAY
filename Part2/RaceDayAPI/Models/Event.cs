namespace RaceDayAPI.Models
{
    public class Event
    {
        public int EventId { get; set; }

        public int OrganiserProfileId { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime EventDate { get; set; }

        public string Location { get; set; } = string.Empty;

        public string EventType { get; set; } = string.Empty;

        public decimal DistanceKm { get; set; }

        public string Status { get; set; } = "Upcoming";

        public OrganiserProfile? OrganiserProfile { get; set; }

        public ICollection<Category> Categories { get; set; }
            = new List<Category>();

        public ICollection<RaceDayAPI.Models.Route> Routes { get; set; }
            = new List<RaceDayAPI.Models.Route>();

        public ICollection<Weather> WeatherRecords { get; set; }
            = new List<Weather>();

        public ICollection<EventImage> Images { get; set; }
            = new List<EventImage>();
    }
}