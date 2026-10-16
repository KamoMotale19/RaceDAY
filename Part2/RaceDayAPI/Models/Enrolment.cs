namespace RaceDayAPI.Models
{
    public class Enrolment
    {
        public int EnrolmentId { get; set; }

        public int ParticipantProfileId { get; set; }

        public int CategoryId { get; set; }

        // Navigation properties
        public ParticipantProfile? ParticipantProfile { get; set; }

        public Category? Category { get; set; }

        public Result? Result { get; set; }

        public ICollection<Payment> Payments { get; set; }
            = new List<Payment>();
    }
}