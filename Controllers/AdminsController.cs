using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GuestSystemBack.Data;
using GuestSystemBack.Models;
using Azure.Core;
using GuestSystemBack.DTOs;
using System.Security.Cryptography;
using System.Security.Claims;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly GuestSystemContext _context;

        public AdminsController(GuestSystemContext context)
        {
            _context = context;
        }

        // GET: api/Admins
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        // GET: api/Admins/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);

            if (admin == null)
            {
                return NotFound();
            }

            return admin;
        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAdmin(int id, AdminDTO request)
        {
            var oldAdmin = await _context.Admins.FindAsync(id);
            if (oldAdmin == null) return BadRequest("Can't find the Admin with given ID");

            /*
            int userID = int.Parse(User.FindFirstValue(ClaimTypes.Name));
            if (User.FindFirstValue(ClaimTypes.Role) == "Courier" && userID != oldDelivery.DeliveryCourierId)
            {
                return BadRequest("You can only edit your own deliveries");
            }*/

            if (request.Name != String.Empty) oldAdmin.Name = request.Name;
            if (request.Email != String.Empty) oldAdmin.Email = request.Email;
            if (request.Password != String.Empty)
            {
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                oldAdmin.PasswordHash = passwordHash;
                oldAdmin.PasswordSalt = passwordSalt;
            }
            await _context.SaveChangesAsync();

            return Ok(oldAdmin);
        }

        // POST: api/Admins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Admin>> PostAdmin(AdminDTO request)
        {
            if (_context.Admins == null)
            {
                return Problem("Entity set 'DataContext.Users'  is null.");
            }

            foreach (Admin user in _context.Admins)
            {
                if (user.Email == request.Email)
                {
                    return BadRequest("User with this email already exists");
                }
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            Admin newAdmin = new()
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                IsSuper = false
            };
            _context.Admins.Add(newAdmin);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdmin", new { id = newAdmin.Id }, newAdmin);
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _context.Admins.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }

            _context.Admins.Remove(admin);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
