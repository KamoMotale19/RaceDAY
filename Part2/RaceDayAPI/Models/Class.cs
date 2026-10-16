namespace RaceDayAPI.Models
{
    public class Class
    {

        public int WeatherId { get; set; }

        public int EventId { get; set; }

        public DateTime RecordedAt { get; set; }

        public decimal Temperature { get; set; }

        public string Conditions { get; set; }

        public decimal Windspeed { get; set; }

        public decimal Humidity { get; set; }

        public String EventName { get; set; }



    }
}
