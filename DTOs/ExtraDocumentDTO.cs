using System.ComponentModel.DataAnnotations;

namespace GuestSystemBack.DTOs
{
    public class ExtraDocumentDTO : IValidatableObject
    {
        public string Title { get; set; } = String.Empty;
        public string Content { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if(Status != "active" && Status != "inactive" && Status != "removed" && Status != "")
            {
                yield return new ValidationResult($"Value:'{Status}' for Status is not allowed.");
            }
        }
    }
}
