using GuestSystemBack.Data;
using GuestSystemBack.Interfaces;
using GuestSystemBack.Models;
using Microsoft.EntityFrameworkCore;

namespace GuestSystemBack.Repositories
{
    public class AdminRepo : IAdminRepo
    {
        private readonly GuestSystemContext _context;

        public AdminRepo(GuestSystemContext context)
        {
            _context = context;
        }

        public Task<int> AddAdmin(Admin admin)
        {
            _context.Add(admin);
            return _context.SaveChangesAsync();
        }

        public bool AdminsExist()
        {
            return _context.Admins != null;
        }

        public async Task<Admin?> GetAdmin(int id)
        {
            return await _context.Admins.FindAsync(id);
        }

        public async Task<List<Admin>> GetAdmins()
        {
            return await _context.Admins.ToListAsync();
        }

        public Task<int> UpdateAdmin(Admin admin)
        {
            _context.Update(admin);
            return _context.SaveChangesAsync();
        }

        public bool AdminWithEmailExists(string email)
        {
            foreach (Admin user in _context.Admins)
            {
                if (user.Email == email)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
