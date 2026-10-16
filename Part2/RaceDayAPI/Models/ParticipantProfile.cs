namespace RaceDayAPI.Models
{
    public class ParticipantProfile
    {
        public int ParticipantProfileId { get; set; }

        public int UserId { get; set; }

        // Navigation properties
        public User? User { get; set; }

        public ICollection<Enrolment> Enrolments { get; set; }
            = new List<Enrolment>();

        public ICollection<EmergencyContact> EmergencyContacts { get; set; }
            = new List<EmergencyContact>();
    }
}