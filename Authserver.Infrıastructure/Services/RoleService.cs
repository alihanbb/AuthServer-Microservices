using Authserver.Application.Interfaces;
using AuthServer.Domain.Entities;
using AuthServer.Domain.Exceptions;
using AuthServerDomain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Authserver.Infrıastructure.Authorization
{
    public class RoleService : IRoleService
    {
        private readonly UserManager<AppUser> userManager;
        private readonly RoleManager<AppRole> roleManager;

        public RoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
        {
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        public async Task<IdentityResult> AssignRoleToUserAsync(Guid userId, string roleName)
        {
            var hasUser = await userManager.FindByIdAsync(userId.ToString());
            if (hasUser is null)
                throw new UserNotFoundException(userId.ToString());
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
                throw new RoleNotFoundException(roleName);
            var isInRole = await userManager.IsInRoleAsync(hasUser, roleName);
            if (isInRole)
                return IdentityResult.Success;
            return await userManager.AddToRoleAsync(hasUser, role.Name);
        }

        public async Task<IdentityResult> CreateRoleAsync(string roleName)
        {
            var addRole = new AppRole
            {
                Name = roleName
            };
            return await roleManager.CreateAsync(addRole);
        }

        public async Task<IdentityResult> RemoveRoleFromUserAsync(Guid userId, string roleName)
        {
        
            var anyUser = await userManager.FindByIdAsync(userId.ToString());
            if (anyUser is null)
                throw new UserNotFoundException(userId.ToString());
            
          
            var role = await roleManager.FindByNameAsync(roleName);
            if (role is null)
                throw new RoleNotFoundException(roleName);
            
           
            var isInRole = await userManager.IsInRoleAsync(anyUser, roleName);
            if (!isInRole)
            {
                return IdentityResult.Success;
            }
            
            return await userManager.RemoveFromRoleAsync(anyUser, role.Name);
        }
    }
}
