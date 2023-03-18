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
using Microsoft.AspNetCore.Authorization;
using System.Data;
using GuestSystemBack.Interfaces;

namespace GuestSystemBack.Controllers
{
    [Route("api/[controller]")]
    //[Authorize(Roles = "super")]
    [ApiController]
    public class AdminsController : ControllerBase
    {
        private readonly IAdminRepo _adminRepo;
        private readonly IUserService _userService;

        public AdminsController(IAdminRepo repo, IUserService userService)
        {
            _adminRepo = repo;
            _userService = userService;
        }

        // GET: api/Admins
        [HttpGet, Authorize(Roles = "super")]
        public async Task<ActionResult<IEnumerable<Admin>>> GetAdmins()
        {
            return await _adminRepo.GetAdmins();
        }

        // GET: api/Admins/5
        [HttpGet("{id}"), Authorize(Roles = "super, regular")]
        public async Task<ActionResult<Admin>> GetAdmin(int id)
        {
            var admin = await _adminRepo.GetAdmin(id);

            if (admin == null) return NotFound("Admin with given ID does not exist");

            if (_userService.GetUserRole() == "regular" && _userService.GetUserId() != admin.Id)
            {
                return BadRequest("You can only view your own account!");
            }

            return admin;
        }

        // PUT: api/Admins/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPatch("{id}"), Authorize(Roles = "super, regular")]
        public async Task<IActionResult> PatchAdmin(int id, AdminDTO request)
        {
            var oldAdmin = await _adminRepo.GetAdmin(id);
            if (oldAdmin == null) return NotFound("Admin with given ID does not exist");

            if (_userService.GetUserRole() == "regular" && _userService.GetUserId() != oldAdmin.Id)
            {
                return BadRequest("You can only edit your own account!");
            }

            //super admin can change all parameters
            if (_userService.GetUserRole() == "super")
            {
                if (request.Name != String.Empty) oldAdmin.Name = request.Name;
                if (request.Email != String.Empty)
                {
                    if (_adminRepo.AdminWithEmailExists(request.Email))
                    {
                        return BadRequest("User with this email already exists");
                    }
                    oldAdmin.Email = request.Email;
                }
            }

            //regular admin can only change their own password
            if (request.Password != String.Empty)
            {
                CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
                oldAdmin.PasswordHash = passwordHash;
                oldAdmin.PasswordSalt = passwordSalt;
            }
            await _adminRepo.UpdateAdmin(oldAdmin);

            return Ok(oldAdmin);
        }

        // POST: api/Admins
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost, Authorize(Roles = "super")]
        public async Task<ActionResult<Admin>> PostAdmin(AdminDTO request)
        {
            if (!_adminRepo.AdminsExist())
            {
                return Problem("Entity set 'DataContext.Admins'  is null.");
            }

            if (_adminRepo.AdminWithEmailExists(request.Email))
            {
                return BadRequest("User with this email already exists");
            }

            CreatePasswordHash(request.Password, out byte[] passwordHash, out byte[] passwordSalt);
            Admin newAdmin = new()
            {
                Name = request.Name,
                Email = request.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                Role = "regular"
            };
            await _adminRepo.AddAdmin(newAdmin);

            return CreatedAtAction("GetAdmin", new { id = newAdmin.Id }, newAdmin);
        }

        // DELETE: api/Admins/5
        [HttpDelete("{id}"), Authorize(Roles = "super")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminRepo.GetAdmin(id);
            if (admin == null)
            {
                return NotFound("Admin with given ID does not exist");
            }
            admin.Role = "removed";
            await _adminRepo.UpdateAdmin(admin);

            return Ok(admin);
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
