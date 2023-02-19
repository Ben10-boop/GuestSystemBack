namespace GuestSystemBack.Models
{
    //The purpose of this class is to solve n:n relation between ExtraDocument and FormSubmission
    public class FormDocument
    {
        public int Id { get; set; }

        public int FormId { get; set; }
        public FormSubmission Form { get; set; } = null;

        public int DocumentId { get; set; }
        public ExtraDocument Document { get; set; } = null;
    }
}