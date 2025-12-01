using AuthServer.Application.DTOs;
using AuthServer.Domain.Entities;

namespace AuthServer.Application.Interfaces;

public interface ITokenService
{
    Task<TokenDto> GenerateToken(AppUser user, IList<string> roles);
    RefreshToken GenerateRefreshToken();
}
