using GuestSystemBack.Models;

namespace GuestSystemBack.Interfaces
{
    public interface IAdminRepo
    {
        public Task<List<Admin>> GetAdmins();
        public Task<Admin?> GetAdmin(int id);
        public Task<int> AddAdmin(Admin admin);
        public Task<int> UpdateAdmin(Admin admin);
        public bool AdminsExist();
        public bool AdminWithEmailExists(string email);
    }
}
