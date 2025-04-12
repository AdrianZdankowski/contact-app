using contact_app.DTO;
using contact_app.Entities;
using Microsoft.AspNetCore.Http;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (await _context.Users.AnyAsync(u => u.email == dto.email))
                return BadRequest(new Response("error", "User already exists."));

            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";

            if (!Regex.IsMatch(dto.email, emailPattern))
            {
                return BadRequest(new Response("error", "Incorrect email format."));
            }

            // at least 8 characters, capital letter, number and special character
            var passwordPattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[\W_]).{8,}$";

            if (!Regex.IsMatch(dto.password, passwordPattern))
            {
                return BadRequest(new Response("error", 
                    "Password must be 8 characters long, contain a capital letter, a number and a special character"));
            }

            var hmac = new HMACSHA512();

            var user = new User
            {
                email = dto.email,
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.password)),
                passwordSalt = hmac.Key
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            var token = CreateToken(user);

            return Ok(new Response("success","User registered successfully", token));
        }


        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDTO dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.email == dto.email);
            if (user == null) 
            {
                return Unauthorized(new Response("error","Bad credentials"));          
            }

            var hmac = new HMACSHA512(user.passwordSalt);
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(dto.password));

            if (!hash.SequenceEqual(user.passwordHash))
            {
                return Unauthorized(new Response("error", "Bad credentials"));
            }

            var token = CreateToken(user);
            return Ok(new Response("success","User logged in successfully",token));
        }


        private string CreateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.id.ToString()),
                new Claim(ClaimTypes.Name, user.email)
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
