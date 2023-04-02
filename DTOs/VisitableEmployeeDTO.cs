using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GuestSystemBack.DTOs
{
    public class VisitableEmployeeDTO : IValidatableObject
    {
        public string Name { get; set; } = String.Empty;
        public string Email { get; set; } = String.Empty;
        public string Status { get; set; } = String.Empty;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Status != "visitable" && Status != "unvisitable" && Status != "")
            {
                yield return new ValidationResult($"Value:'{Status}' for Status is not allowed.");
            }
            Regex rg = new Regex(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$");
            if (Email != String.Empty && !rg.IsMatch(Email))
            {
                yield return new ValidationResult($"Value:'{Email}' is not a proper email address");
            }
        }
    }
}
