using System.Text.Json.Serialization;

namespace GuestSystemBack.Models
{
    public class VisitableEmployee
    {
        public int Id { get; set; }
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Status { get; set; } = "visitable"; //visitable, unvisitable
        [JsonIgnore]
        public ICollection<FormSubmission> FormSubmissions { get; set; } = null;
    }
}