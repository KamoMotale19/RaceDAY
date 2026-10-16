namespace RaceDayAPI.Models
{
    public class EmergencyContact
    {
        public int EmergencyContactId { get; set; }

        public int ParticipantProfileId { get; set; }

        public string ContactName { get; set; } = string.Empty;

        public string Relationship { get; set; } = string.Empty;

        public string PhoneNumber { get; set; } = string.Empty;

        // Navigation property
        public ParticipantProfile ParticipantProfile { get; set; } = null!;
    }
}