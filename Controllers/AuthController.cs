using GuestSystemBack.Data;
using GuestSystemBack.DTOs;
using GuestSystemBack.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly GuestSystemContext _context;
        private readonly IConfiguration _configuration;
        public AuthController(GuestSystemContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(AdminDTO request)
        {
            if (_context.Admins == null) return NotFound("No admins exist");

            Admin foundAdmin = null;
            foreach (var admin in _context.Admins)
            {
                if(admin.Email == request.Email)
                {
                    foundAdmin = admin;
                    break;
                }
            }

            if (foundAdmin == null || !VerifyPassWordHash(request.Password, foundAdmin.PasswordHash, foundAdmin.PasswordSalt))
                return BadRequest("Wrong credentials");

            string token = CreateToken(foundAdmin);

            return Ok(token);
        }

        private string CreateToken(Admin admin)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, admin.Email),
                new Claim(ClaimTypes.Name, $"{admin.Id}"),
                new Claim(ClaimTypes.Role, admin.Role),
                new Claim(ClaimTypes.Expiration, new DateTimeOffset(DateTime.Now.AddHours(2)).ToUnixTimeMilliseconds().ToString())
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: creds
                );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);
            return jwt;
        }

        private bool VerifyPassWordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
