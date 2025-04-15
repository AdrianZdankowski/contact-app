using System.ComponentModel.DataAnnotations;

namespace contact_app.Entities
{
    public class Subcategory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
