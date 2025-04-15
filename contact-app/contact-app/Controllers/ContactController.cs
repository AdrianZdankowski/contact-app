using contact_app.Database;
using contact_app.DTO;
using contact_app.DTO.Contact;
using contact_app.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;
using System.Text;

namespace contact_app.Controllers
{
    [Route("api/[controller]")]
    public class ContactController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        // Pobiera wszystkie kontakty bez szczegółowych informacji /api/contact
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetContactsDTO>>> GetAllContacts()
        {
            var contacts = await _context.Contacts
                .Include(c => c.Category) // Ładowanie kategorii
                .Include(c => c.Subcategory) // Ładowanie subkategorii
                .Select(c => new GetContactsDTO
                {
                    Id = c.Id,
                    FirstName = c.FirstName,
                    LastName = c.LastName,
                    Email = c.Email,
                    PhoneNumber = c.PhoneNumber
                })
                .ToListAsync();

            return Ok(contacts);
        }

        // Pobiera szczegóły danego kontatku po jego ID /api/contact/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<GetContactDTO>> GetContact(int id)
        {
            var contact = await _context.Contacts
                .Include(c => c.Category) // Ładowanie kategorii
                .Include(c => c.Subcategory) // Ładowanie subkategorii
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound();

            var dto = new GetContactDTO
            {
                Id = contact.Id,
                FirstName = contact.FirstName,
                LastName = contact.LastName,
                Email = contact.Email,
                Category = contact.Category?.Name, 
                Subcategory = contact.Subcategory?.Name, 
                PhoneNumber = contact.PhoneNumber,
                BirthDate = contact.BirthDate
            };

            return Ok(dto);
        }


        // Tworzenie nowego kontatku, wymaga autoryzacji z tokenem JWT /api/contact
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Contact>> CreateContact([FromBody] PostContactDTO contactDTO)
        {
            if (contactDTO == null)
            {
                return BadRequest(new Response("error", "Invalid input data"));
            }

            // Weryfikacja kategorii
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Name == contactDTO.Category);

            if (category == null)
            {
                return BadRequest(new Response("error", "Invalid Category."));
            }

            // Weryfikacja lub utworzenie subkategorii, jeśli podano
            Subcategory? subcategory = null;
            if (!string.IsNullOrEmpty(contactDTO.SubCategory))
            {
                if (contactDTO.Category == "Inny")
                {
                    // Dla kategorii "Inny" subkategorie mogą być dynamicznie tworzone
                    subcategory = await _context.Subcategories
                        .FirstOrDefaultAsync(sc => sc.Name == contactDTO.SubCategory && sc.CategoryId == category.Id);

                    if (subcategory == null)
                    {
                        subcategory = new Subcategory
                        {
                            Name = contactDTO.SubCategory,
                            CategoryId = category.Id
                        };
                        _context.Subcategories.Add(subcategory);
                        await _context.SaveChangesAsync();
                    }
                }
                else
                {
                    // W pozostałych przypadkach subkategoria musi istnieć
                    subcategory = await _context.Subcategories
                        .FirstOrDefaultAsync(sc => sc.Name == contactDTO.SubCategory && sc.CategoryId == category.Id);

                    if (subcategory == null)
                    {
                        return BadRequest(new Response("error", "Invalid Subcategory."));
                    }
                }
            }

            // Hashowanie hasła z solą
            using var hmac = new HMACSHA512();
            var passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(contactDTO.Password));
            var passwordSalt = hmac.Key;

            // Tworzenie kontatku na podstawie PostContactDTO
            var contact = new Contact
            {
                FirstName = contactDTO.FirstName,
                LastName = contactDTO.LastName,
                Email = contactDTO.Email,
                PhoneNumber = contactDTO.PhoneNumber,
                BirthDate = contactDTO.BirthDate,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                CategoryId = category.Id,
                Category = category,
                SubcategoryId = subcategory?.Id,
                Subcategory = subcategory
            };

            // Dodanie kontaktu do bazy danych
            _context.Contacts.Add(contact);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetContact), new { id = contact.Id }, new Response("success", "Contact created successfully"));
        }

        // Aktualizacja kontaktu, wymaga autoryzacji i podania starego hasła kontaktu
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Response>> UpdateContact(int id, [FromBody] PutContactDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new Response("error", "Invalid input data"));

            var contact = await _context.Contacts
                .Include(c => c.Category)
                .Include(c => c.Subcategory)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contact == null)
                return NotFound(new Response("error", "Contact not found"));

            // Sprawdzenie starego hasła
            using var oldHmac = new HMACSHA512(contact.PasswordSalt);
            var oldHash = oldHmac.ComputeHash(Encoding.UTF8.GetBytes(dto.OldPassword));

            if (!oldHash.SequenceEqual(contact.PasswordHash))
                return Unauthorized(new Response("error", "Invalid old password"));

            // Zmiana hasła
            using var newHmac = new HMACSHA512();
            contact.PasswordSalt = newHmac.Key;
            contact.PasswordHash = newHmac.ComputeHash(Encoding.UTF8.GetBytes(dto.NewPassword));

            // Weryfikacja kategorii
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == dto.Category);
            if (category == null)
                return BadRequest(new Response("error", "Invalid category"));

            contact.CategoryId = category.Id;
            contact.Category = category;

            // Obsługa subkategorii
            if (!string.IsNullOrEmpty(dto.SubCategory))
            {
                if (dto.Category == "Inny")
                {
                    var subcategory = await _context.Subcategories
                        .FirstOrDefaultAsync(sc => sc.Name == dto.SubCategory && sc.CategoryId == category.Id);

                    if (subcategory == null)
                    {
                        subcategory = new Subcategory
                        {
                            Name = dto.SubCategory,
                            CategoryId = category.Id
                        };
                        _context.Subcategories.Add(subcategory);
                        await _context.SaveChangesAsync();
                    }

                    contact.SubcategoryId = subcategory.Id;
                    contact.Subcategory = subcategory;
                }
                else
                {
                    var subcategory = await _context.Subcategories
                        .FirstOrDefaultAsync(sc => sc.Name == dto.SubCategory && sc.CategoryId == category.Id);

                    if (subcategory == null)
                        return BadRequest(new Response("error", "Invalid subcategory"));

                    contact.SubcategoryId = subcategory.Id;
                    contact.Subcategory = subcategory;
                }
            }
            else
            {
                // Brak subkategorii np. "Prywatny"
                contact.SubcategoryId = null;
                contact.Subcategory = null;
            }

            contact.FirstName = dto.FirstName;
            contact.LastName = dto.LastName;
            contact.Email = dto.Email;
            contact.PhoneNumber = dto.PhoneNumber;
            contact.BirthDate = dto.BirthDate;

            await _context.SaveChangesAsync();

            return Ok(new Response("success", "Contact updated"));
        }

        // Usuwanie kontaktu – wymaga hasła kontaktu i autoryzacji
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Response>> DeleteContact(int id, [FromBody] DeleteContactDTO dto)
        {
            var contact = await _context.Contacts.FirstOrDefaultAsync(c => c.Id == id);

            if (contact is null)
                return NotFound(new Response("error", "Contact not found."));

            // Sprawdzenie poprawności hasła
            using var hmac = new HMACSHA512(contact.PasswordSalt);
            var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!computedHash.SequenceEqual(contact.PasswordHash))
                return BadRequest(new Response("error", "Invalid password."));

            _context.Contacts.Remove(contact);
            await _context.SaveChangesAsync();

            return Ok(new Response("success", "Contact deleted successfully."));
        }
    }
}
