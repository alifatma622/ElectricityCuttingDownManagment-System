using ElectricityCuttingDown.WebPortal.Models.Entities;

namespace ElectricityCuttingDown.WebPortal.Services
{
    public interface IAuthService
    {
        Task<User> AuthenticateAsync(string username, string password);
        Task<User> GetUserByUsernameAsync(string username);
    }
}
