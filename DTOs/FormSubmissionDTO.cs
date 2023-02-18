using GuestSystemBack.Models;

namespace GuestSystemBack.DTOs
{
    public class FormSubmissionDTO
    {
        public string Name { get; set; } = String.Empty;
        public string? Email { get; set; }
        public string VisitPurpose { get; set; } = String.Empty;
        public string Signature { get; set; } = String.Empty; //idk what this
        public DateTime EntranceTime { get; set; } = DateTime.Now;
        public DateTime? DepartureTime { get; set; }

        public int VisiteeId { get; set; } = -1;
        public string WifiAccessStatus { get; set; } = "not requested"; //"not requested"; "granted";
        public int? SubmitterId { get; set; }
    }
}
