using Microsoft.AspNetCore.Identity;

namespace Authserver.Application.Interfaces;

public interface IRoleService
{
    Task<IdentityResult> CreateRoleAsync(string roleName); 
    Task<IdentityResult> AssignRoleToUserAsync(Guid userId, string roleName);
    Task<IdentityResult> RemoveRoleFromUserAsync(Guid userId, string roleName);
}

