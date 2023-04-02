using GuestSystemBack.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace GuestSystemBack.DTOs
{
    public class FormSubmissionDTO : IValidatableObject
    {
        public string Name { get; set; } = String.Empty;
        public string? Email { get; set; }
        public string VisitPurpose { get; set; } = String.Empty;
        public string? Signature { get; set; } //idk what this
        public DateTime? EntranceTime { get; set; }
        public DateTime? DepartureTime { get; set; }

        public int VisiteeId { get; set; } = -1;
        public string WifiAccessStatus { get; set; } = "not requested"; //"not requested"; "granted";

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (WifiAccessStatus != "not requested" && WifiAccessStatus != "granted" && WifiAccessStatus != "")
            {
                yield return new ValidationResult($"Value:'{WifiAccessStatus}' for WifiAccessStatus is not allowed.");
            }
            Regex rg = new Regex(@"^[a-zA-Z0-9+_.-]+@[a-zA-Z0-9.-]+$");
            if (Email != String.Empty && Email != null && !rg.IsMatch(Email))
            {
                yield return new ValidationResult($"Value:'{Email}' is not a proper email address");
            }
        }
    }
}
