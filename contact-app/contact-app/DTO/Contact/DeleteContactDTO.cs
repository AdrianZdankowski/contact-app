using System.ComponentModel.DataAnnotations;

namespace contact_app.DTO.Contact
{
    public record class DeleteContactDTO
    {
        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain a capital letter, a number and a special character")]
        public string Password { get; set; }
    }
}
