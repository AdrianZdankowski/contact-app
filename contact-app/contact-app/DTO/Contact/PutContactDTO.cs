using System.ComponentModel.DataAnnotations;

namespace contact_app.DTO.Contact
{
    public record class PutContactDTO
    {
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [MaxLength(9)]
        public string PhoneNumber { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string Category { get; set; } = string.Empty;
        public string? SubCategory { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain a capital letter, a number and a special character")]
        public string OldPassword { get; set; } = string.Empty;

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain a capital letter, a number and a special character")]
        public string NewPassword { get; set; } = string.Empty; 
    }
}
