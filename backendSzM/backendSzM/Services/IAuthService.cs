using backendSzM.DTOs;
using backendSzM.Models;

namespace backendSzM.Services
{
    public interface IAuthService
    {
        Task<UserData?> RegisterAsync(UserDataDTO request);
        Task<string?> LoginAsync(UserDataDTO request);
    }
}
