using contact_app.Entities;
using System.Security.Cryptography;
using System.Text;

namespace contact_app.Database
{
    public class DataSeeder
    {
        private readonly AppDbContext _context;

        public DataSeeder(AppDbContext context)
        {
            _context = context;
        }

        // Metoda uruchamiana przy starcie aplikacji
        public void Seed()
        {
            // Tworzy bazę danych jeśli jeszcze nie istnieje
            _context.Database.EnsureCreated();

            // Dodaje kategorie, jeśli nie istnieją
            if (!_context.Categories.Any())
            {
                AddCategories();
            }

            // Dodaje subkategorie, jeśli nie istnieją
            if (!_context.Subcategories.Any())
            {
                AddSubcategories();
            }

            // Dodaje kontakty, jeśli nie istnieją
            if (!_context.Contacts.Any())
            {
                AddContacts();
            }
        }

        // Dodaje kategorie domyślne
        private void AddCategories()
        {
            var categories = new List<Category>
            {
                new Category { Name = "Służbowy" },
                new Category { Name = "Prywatny" },
                new Category { Name = "Inny" }
            };

            _context.Categories.AddRange(categories);
            _context.SaveChanges();
        }

        // Dodaje subkategorie przypisane do konkretnych kategorii
        private void AddSubcategories()
        {
            // Pobieranie ID wybranych kategorii
            var sluzbowyCategoryId = _context.Categories.First(c => c.Name == "Służbowy").Id;
            var innyCategoryId = _context.Categories.First(c => c.Name == "Inny").Id;

            var subcategories = new List<Subcategory>
            {
                // Subkategorie dla "Służbowy"
                new Subcategory { Name = "Klient", CategoryId = sluzbowyCategoryId },
                new Subcategory { Name = "Szef", CategoryId = sluzbowyCategoryId },

                // Subkategorie dla "Inny"
                new Subcategory { Name = "Znajomy z siłowni", CategoryId = innyCategoryId },
                new Subcategory { Name = "Partner biznesowy", CategoryId = innyCategoryId },
                new Subcategory { Name = "Kolega", CategoryId = innyCategoryId }
            };

            _context.Subcategories.AddRange(subcategories);
            _context.SaveChanges();
        }

        // Dodaje 5 przykładowych kontaktów
        private void AddContacts()
        {
            var contacts = new List<Contact>();

            // Pomocnicza metoda do tworzenia kontaktów
            void AddContact(string firstName, string lastName, string email, string password, string phone, DateOnly birthDate, string category, string subcategory)
            {
                //Hashowanie hasła
                using var hmac = new HMACSHA512();
                var categoryEntity = _context.Categories.FirstOrDefault(c => c.Name == category);

                if (categoryEntity == null)
                {
                    return; // Jeśli nie znaleziono kategorii - koniec
                }

                Subcategory? subcategoryEntity = null;

                if (category == "Inny" && !string.IsNullOrEmpty(subcategory))
                {
                    // Jeśli kategoria to "Inny", znajduje subkategorię powiązaną z tą kategorią
                    subcategoryEntity = _context.Subcategories.FirstOrDefault(sc => sc.Name == subcategory && sc.CategoryId == categoryEntity.Id);
                }
                else if (category != "Inny" && !string.IsNullOrEmpty(subcategory))
                {
                    // W innych przypadkach też przypisuje subkategorię zgodnie z kategorią
                    subcategoryEntity = _context.Subcategories.FirstOrDefault(sc => sc.Name == subcategory && sc.CategoryId == categoryEntity.Id);
                }

                //Tworzenie obiektu kontaktu
                contacts.Add(new Contact
                {
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email,
                    PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
                    PasswordSalt = hmac.Key,
                    PhoneNumber = phone,
                    BirthDate = birthDate,
                    CategoryId = categoryEntity.Id,
                    Category = categoryEntity,
                    SubcategoryId = subcategoryEntity?.Id,
                    Subcategory = subcategoryEntity
                });
            }

            // Tworzenie przykładowych kontaktów
            AddContact("Jan", "Kowalski", "jan.kowalski@example.com", "Haslo123!", "123456789", new DateOnly(2000, 5, 12), "Służbowy", "Klient");
            AddContact("Anna", "Nowak", "anna.nowak@example.com", "Nowak@2023", "987654321", new DateOnly(2005, 2, 28), "Prywatny", "");
            AddContact("Piotr", "Wiśniewski", "piotr.w@example.com", "P@ssword123", "555123456", new DateOnly(1992, 7, 19), "Inny", "Znajomy z siłowni");
            AddContact("Maria", "Zielińska", "maria.z@example.com", "M@riaZiel!", "444567890", new DateOnly(1998, 11, 5), "Służbowy", "Szef");
            AddContact("Tomasz", "Grabowski", "t.grabowski@example.com", "Tomek2024*", "321654987", new DateOnly(1984, 3, 22), "Inny", "Partner biznesowy");

            // Zapis kontaktów do bazy
            _context.Contacts.AddRange(contacts);
            _context.SaveChanges();
        }
    }
}
