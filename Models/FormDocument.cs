namespace GuestSystemBack.Models
{
    public class FormDocument
    {
        public int Id { get; set; }

        public int FormSubmissionId { get; set; }
        public FormSubmission Form { get; set; } = null;

        public int ExtraDocumentId { get; set; }
        public ExtraDocument Document { get; set; } = null;
    }
}