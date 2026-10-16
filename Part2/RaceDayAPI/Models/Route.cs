namespace RaceDayAPI.Models
{
    public class Route
    {
        public int RouteId { get; set; }

        public int EventId { get; set; }

        public decimal DistanceKm { get; set; }

        public string RouteName { get; set; } = string.Empty;

        public string StartLocation { get; set; } = string.Empty;

        public string FinishLocation { get; set; } = string.Empty;

        public string RouteUrl { get; set; } = string.Empty;

        public decimal ElevationGain { get; set; }

        public Event? Event { get; set; }
    }
}