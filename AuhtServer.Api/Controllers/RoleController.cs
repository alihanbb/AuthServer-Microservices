using Authserver.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AuhtServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "roles.read")] // CRUD tabanlı izin sistemi

    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly ILogger<RoleController> _logger;
        
        public RoleController(
            IRoleService roleService,
            ILogger<RoleController> logger)
        {
            _roleService = roleService;
            _logger = logger;
        }
        
        [HttpPost("create")]
        [Authorize(Policy = "roles.create")] // CRUD tabanlı izin sistemi
        public async Task<IActionResult> CreateRole(string roleName)
        {
            _logger.LogInformation("Creating role: {RoleName}", roleName);
            var result = await _roleService.CreateRoleAsync(roleName);
            if (result.Succeeded)
                return Ok($"Rol {roleName} başarıyla oluşturuldu.");
            return BadRequest(result.Errors);
        }
        
        [HttpPost("assign")]
        [Authorize(Policy = "roles.update")] // Rol atama = güncelleme
        public async Task<IActionResult> AssignRoleToUser(Guid userId, string roleName)
        {
            _logger.LogInformation("Assigning role {RoleName} to user {UserId}", roleName, userId);
            var result = await _roleService.AssignRoleToUserAsync(userId, roleName);
            if (result.Succeeded)
                return Ok($"Kullanıcıya {roleName} rolü başarıyla atandı.");
            return BadRequest(result.Errors);
        }
        
        [HttpPost("remove")]
        [Authorize(Policy = "roles.update")] // Rol kaldırma = güncelleme
        public async Task<IActionResult> RemoveRoleFromUser(Guid userId, string roleName)
        {
            _logger.LogInformation("Removing role {RoleName} from user {UserId}", roleName, userId);
            var result = await _roleService.RemoveRoleFromUserAsync(userId, roleName);
            if (result.Succeeded)
                return Ok($"Kullanıcıdan {roleName} rolü başarıyla kaldırıldı.");
            return BadRequest(result.Errors);
        }
    }
}
