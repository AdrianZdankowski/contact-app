using contact_app.Entities;
using Microsoft.EntityFrameworkCore;

namespace contact_app.Database
{
    public class AppDbContext : DbContext
    {
        // Konstruktor przekazujący opcje do bazy
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Reprezentacja tabel w bazie danych
        public DbSet<User> Users { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; } 
        public DbSet<Subcategory> Subcategories { get; set; }

        // Konfiguracja modeli i relacji między nimi
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email użytkownika musi być unikalny
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            // Email kontaktu musi być unikalny
            modelBuilder.Entity<Contact>()
                .HasIndex(contact => contact.Email)
                .IsUnique();

            // Kontakt musi mieć kategorię, ale jeśli kategoria zostanie usunięta, kontakt nie zostanie usunięty
            modelBuilder.Entity<Contact>()
                .HasOne(contact => contact.Category)      // Kontakt ma jedną kategorię
                .WithMany()                               // Kategoria może być przypisana do wielu kontaktów
                .HasForeignKey(contact => contact.CategoryId) // Klucz obcy w tabeli Contact
                .OnDelete(DeleteBehavior.Restrict);       // Nie pozwala na usunięcie kategorii, jeśli jest powiązana

            // Subkategoria może być przypisana do kontaktu, ale jeśli zostanie usunięta, kontakt nie zostanie usunięty
            modelBuilder.Entity<Contact>()
                .HasOne(contact => contact.Subcategory)   // Kontakt może mieć subkategorię
                .WithMany()                               // Subkategoria może być użyta przez wiele kontaktów
                .HasForeignKey(contact => contact.SubcategoryId) // Klucz obcy
                .OnDelete(DeleteBehavior.SetNull);        // Po usunięciu subkategorii pole SubcategoryId w Contact będzie null

            // Wywołanie domyślnej konfiguracji bazowej
            base.OnModelCreating(modelBuilder);
        }
    }
}
