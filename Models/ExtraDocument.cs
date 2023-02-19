using System.Text.Json.Serialization;

namespace GuestSystemBack.Models
{
    public class ExtraDocument
    {
        public int Id { get; set; }
        public string Title { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public string Status { get; set; } = "active"; //active; inactive; removed

        [JsonIgnore]
        public ICollection<FormDocument> FormDocuments { get; set; } = null;
    }
}