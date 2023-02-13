namespace GuestSystemBack.Models
{
    public class ExtraDocument
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;

        public ICollection<FormDocument> FormDocuments { get; set; } = null;
    }
}