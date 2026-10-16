namespace RaceDayAPI.Models
{
    public class Weather
    {
        public int WeatherId { get; set; }

        public int EventId { get; set; }

        public DateTime RecordedAt { get; set; }

        public decimal Temperature { get; set; }

        public string Condition { get; set; } = string.Empty;

        public decimal WindSpeed { get; set; }

        public int Humidity { get; set; }

        // Navigation property
        public Event? Event { get; set; }
    }
}