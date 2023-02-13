namespace GuestSystemBack.Models
{
    public class Admin
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Password { get; set; } = String.Empty;
        public bool IsSuper { get; set; }
        public ICollection<FormSubmission> FormSubmissions { get; set; } = null;
    }
}
