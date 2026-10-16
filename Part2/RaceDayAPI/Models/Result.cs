namespace RaceDayAPI.Models
{
    public class Result
    {
        public int ResultId { get; set; }

        public int EnrolmentId { get; set; }

        public TimeSpan FinishTime { get; set; }

        public int Position { get; set; }

        public decimal AveragePace { get; set; }

        public string Status { get; set; } = string.Empty;

        // Navigation property
        public Enrolment? Enrolment { get; set; }
    }
}