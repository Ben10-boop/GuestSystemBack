namespace GuestSystemBack.Models
{
    public class VisitableEmployee
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public ICollection<FormSubmission> FormSubmissions { get; set; } = null;
    }
}