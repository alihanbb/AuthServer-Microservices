using AuthServer.Application.Interfaces;
using AuthServer.Domain.Entities;
using AuthServer.Domain.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace AuthServer.Infrastructure.Services;

public class RoleService : IRoleService
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<AppRole> _roleManager;

    public RoleService(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task<IdentityResult> CreateRoleAsync(string roleName)
    {
        var role = new AppRole { Name = roleName };
        return await _roleManager.CreateAsync(role);
    }

    public async Task<IdentityResult> AssignRoleToUserAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UserNotFoundException(userId.ToString());

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            throw new RoleNotFoundException(roleName);

        var isInRole = await _userManager.IsInRoleAsync(user, roleName);
        if (isInRole)
            return IdentityResult.Success;

        return await _userManager.AddToRoleAsync(user, roleName);
    }

    public async Task<IdentityResult> RemoveRoleFromUserAsync(Guid userId, string roleName)
    {
        var user = await _userManager.FindByIdAsync(userId.ToString());
        if (user == null)
            throw new UserNotFoundException(userId.ToString());

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            throw new RoleNotFoundException(roleName);

        var isInRole = await _userManager.IsInRoleAsync(user, roleName);
        if (!isInRole)
            return IdentityResult.Success;

        return await _userManager.RemoveFromRoleAsync(user, roleName);
    }
}
