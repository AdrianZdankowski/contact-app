using System.ComponentModel.DataAnnotations;

namespace contact_app.Entities
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(8)]
        public string Name { get; set; }

        
    }
}
