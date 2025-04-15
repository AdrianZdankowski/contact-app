using System.ComponentModel.DataAnnotations;

namespace contact_app.DTO.Authorization
{
    public record class LoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email {  get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain a capital letter, a number and a special character")]
        public string Password { get; set; }
    }
}
