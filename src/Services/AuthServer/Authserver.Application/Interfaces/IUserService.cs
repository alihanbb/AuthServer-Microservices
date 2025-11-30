using AuthServerDomain.Entities;

namespace Authserver.Application.Interfaces
{
    public interface IUserService
    {
        Task<AppUser?> GetByUsernameAsync(string username);
        Task<AppUser?> GetByIdAsync(Guid id);
        Task AddUserAsync(AppUser user, string password);
        Task UpdateUserAsync(object user);
        Task<AppUser> GetByRefreshTokenAsync(string refreshToken);
    }
}
