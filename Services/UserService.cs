using GuestSystemBack.Interfaces;
using System.Security.Claims;

namespace GuestSystemBack.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserService(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public int GetUserId()
        {
            if(_contextAccessor.HttpContext != null)
            {
                return int.Parse(_contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Name));
            }
            return -1;
        }
        public string GetUserRole()
        {
            if (_contextAccessor.HttpContext != null)
            {
                return _contextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Role);
            }
            return String.Empty;
        }
    }
}
