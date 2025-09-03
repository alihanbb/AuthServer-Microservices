using Authserver.Application.Interfaces;
using Authserver.Infrıastructure.Persistence.Context;
using AuthServerDomain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Authserver.Infrastructure.Services
{
    public class AppUserRepository(AuthDbContext authDbContext) : IUserService
    {
        public async Task AddUserAsync(AppUser user, string password)
        {
            await authDbContext.Users.AddAsync(user);
            await authDbContext.SaveChangesAsync();
        }

        public async Task<AppUser?> GetByIdAsync(Guid id)
        {
            return await authDbContext.Users
                 .Include(u => u.RefreshTokens)
                 .FirstOrDefaultAsync(u => u.Id == id);
        }


        public async Task<AppUser?> GetByRefreshTokenAsync(string refreshToken)
        {
            return await authDbContext.Users
                .Include(u => u.RefreshTokens)
                .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
        }



        public async Task<AppUser?> GetByUsernameAsync(string username)
        {
            return await authDbContext.Users
                 .Include(u => u.RefreshTokens)
                 .FirstOrDefaultAsync(u => u.UserName == username);
        }

        public async Task UpdateUserAsync(object user)
        {
            if (user is AppUser appUser)
            {
                authDbContext.Users.Update(appUser);
                await authDbContext.SaveChangesAsync();
            }
        }
    }
}
