using System.ComponentModel.DataAnnotations;

namespace contact_app.DTO.Contact
{
    public record class GetContactDTO
    {
        public int Id { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Category { get; set; }

        public string? Subcategory { get; set; }

        public string PhoneNumber { get; set; }

        public DateOnly BirthDate { get; set; }
    }
}
