using System.Text.Json.Serialization;

namespace GuestSystemBack.Models
{
    public class FormSubmission
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string? Email { get; set; }
        public string VisitPurpose { get; set; } = String.Empty;
        public string? Signature { get; set; } //idk what this
        public DateTime EntranceTime { get; set; } = DateTime.Now;
        public DateTime? DepartureTime { get; set; }

        public int VisiteeId { get; set; }
        [JsonIgnore]
        public VisitableEmployee Visitee { get; set; } = null;
        public string WifiAccessStatus { get; set; } = "not requested"; //"not requested"; "granted";

        [JsonIgnore]
        public ICollection<FormDocument> FormDocuments { get; set; } = null;

    }
}