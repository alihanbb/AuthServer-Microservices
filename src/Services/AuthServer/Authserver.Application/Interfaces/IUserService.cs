using AuthServer.Domain.Entities;

namespace AuthServer.Application.Interfaces;

public interface IUserService
{
    Task<AppUser?> GetByUsernameAsync(string username);
    Task<AppUser?> GetByIdAsync(Guid id);
    Task AddUserAsync(AppUser user, string password);
    Task UpdateUserAsync(AppUser user);
    Task<AppUser?> GetByRefreshTokenAsync(string refreshToken);
}
