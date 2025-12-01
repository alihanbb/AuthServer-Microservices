using AuthServer.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RoleController : ControllerBase
{
    private readonly IRoleService _roleService;

    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> CreateRole([FromBody] string roleName)
    {
        var result = await _roleService.CreateRoleAsync(roleName);
        if (result.Succeeded)
            return Ok(new { message = $"Rol '{roleName}' oluşturuldu" });
        return BadRequest(result.Errors);
    }

    [HttpPost("assign")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignRole(Guid userId, string roleName)
    {
        var result = await _roleService.AssignRoleToUserAsync(userId, roleName);
        if (result.Succeeded)
            return Ok(new { message = "Rol atandı" });
        return BadRequest(result.Errors);
    }

    [HttpPost("remove")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> RemoveRole(Guid userId, string roleName)
    {
        var result = await _roleService.RemoveRoleFromUserAsync(userId, roleName);
        if (result.Succeeded)
            return Ok(new { message = "Rol kaldırıldı" });
        return BadRequest(result.Errors);
    }
}
