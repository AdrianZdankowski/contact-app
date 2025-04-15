using System.ComponentModel.DataAnnotations;

namespace contact_app.DTO.Contact
{
    public record class PostContactDTO
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string Category { get; set; } = string.Empty;
        public string? SubCategory { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MaxLength(9)]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        public DateOnly BirthDate { get; set; }

        [Required]
        [MinLength(8)]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$",
            ErrorMessage = "Password must be at least 8 characters long, contain a capital letter, a number and a special character")]
        public string Password { get; set; } = string.Empty;
    }
}
