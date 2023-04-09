using GuestSystemBack.DTOs;

namespace GuestSystemBack.Interfaces
{
    public interface ICiscoApiService
    {
        public List<GuestUser> GetCurrentWifiUsers();
        public void PostWifiUser(GuestUserDTO guestUser);
    }
}
