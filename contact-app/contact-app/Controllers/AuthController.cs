using contact_app.Database;
using contact_app.DTO;
using contact_app.DTO.Authorization;
using contact_app.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace contact_app.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // Rejestracja użytkownika /api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            // Sprawdzenie czy użytkownik o danym adresie email istnieje w bazie, jeśli tak - BadRequest
            if (await _context.Users.AnyAsync(u => u.Email == dto.Email))
                return BadRequest(new Response("error", "User already exists."));


            // Walidacja adresu e-mail przy pomocy wyrażenia regularnego
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(dto.Email, emailPattern))
            {
                return BadRequest(new Response("error", "Incorrect email format."));
            }

            // Hasło musi zawierać co najmniej 8 znaków, wielką literę, cyfrę i znak specjalny
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";

            if (!Regex.IsMatch(dto.Password, passwordPattern))
            {
                return BadRequest(new Response("error", 
                    "Password must be 8 characters long, contain a capital letter, a number and a special character"));
            }

            // Tworzenie hasha i soli dla hasła
            var hmac = new HMACSHA512();

            var user = new User
            {
                Email = dto.Email,
                PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password)), // hashowanie hasła
                PasswordSalt = hmac.Key // zapamiętanie dodanej soli
            };

            // Dodanie użytkownika do bazy danych
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Utworzenie tokenu JWT
            var token = CreateToken(user);

            // Zwrócenie tokenu użytkownikowi
            return Ok(new Response("success","User registered successfully", token));
        }

        // Logowanie użytkownika api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            // Sprawdzenie czy użytkownik już jest zalogowany
            var authHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();

            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                return BadRequest(new Response("error", "You are already logged in."));
            }

            // Wyszukiwanie użytkownika w bazie po adresie email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null) 
            {
                return Unauthorized(new Response("error","Bad credentials"));          
            }

            // Sprawdzenie hasła
            var hmac = new HMACSHA512(user.PasswordSalt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.Password));

            if (!hash.SequenceEqual(user.PasswordHash))
            {
                return Unauthorized(new Response("error", "Bad credentials"));
            }

            // Jeśli hasło poprawne, tworzy i zwraca token
            var token = CreateToken(user);
            return Ok(new Response("success","User logged in successfully",token));
        }

        // Metoda tworząca token JWT
        private string CreateToken(User user)
        {
            // Tworzy listę claims czyli danych zaszytych w tokenie
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Email)
            };

            // Tworzymy klucz bezpieczeństwa na podstawie ustawień z appsettings
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            // Tworzenie tokenu
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], // nadawca
                audience: _configuration["Jwt:Audience"], // odbiorca
                claims: claims,
                expires: DateTime.Now.AddMinutes(15), // ważność tokenu 15 minut
                signingCredentials: creds // podpis
            );

            // Zwrócenie tokenu jako string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
