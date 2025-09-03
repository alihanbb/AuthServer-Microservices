using Authserver.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuhtServer.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("{username}")]
        public async Task<IActionResult> GetUser(string username)
        {
            var user = await userService.GetByUsernameAsync(username);
            if (user == null) 
                return NotFound();
            return Ok(user);
        }
    }
}
