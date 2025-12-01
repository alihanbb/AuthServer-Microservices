using AuthServer.Redis.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class TokenController : ControllerBase
{
    private readonly ITokenRevocationService _tokenRevocationService;

    public TokenController(ITokenRevocationService tokenRevocationService)
    {
        _tokenRevocationService = tokenRevocationService;
    }

    [HttpPost("revoke")]
    public async Task<IActionResult> RevokeToken([FromBody] string token)
    {
        await _tokenRevocationService.RevokeTokenAsync(token, TimeSpan.FromHours(1));
        return Ok(new { message = "Token kara listeye eklendi" });
    }

    [HttpPost("check")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckToken([FromBody] string token)
    {
        var isRevoked = await _tokenRevocationService.IsTokenRevokedAsync(token);
        return Ok(new { isRevoked, message = isRevoked ? "Token iptal edilmiş" : "Token geçerli" });
    }
}
