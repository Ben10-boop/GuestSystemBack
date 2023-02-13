namespace GuestSystemBack.Models
{
    public class FormSubmission
    {
        public int Id { get; set; }
        public string GuestName { get; set; } = String.Empty;
        public string VisitPurpose { get; set; } = String.Empty;
        public string Signature { get; set; } = String.Empty; //idk what this
        public DateTime SubmissionTime { get; set; }
        public DateTime VisitEndTime { get; set; }

        public int VisiteeId { get; set; }
        public VisitableEmployee Visitee { get; set; } = null;

        public bool WasSubmitByAdmin { get; set; }
        public int? SubmitterId { get; set; }
        public Admin? Submitter { get; set; }

        public ICollection<FormDocument> FormDocuments { get; set; } = null;

    }
}