using Authserver.Application.DTOs;
using AuthServerDomain.Entities;

namespace Authserver.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> GenerateToken(AppUser user, IList<string> roles);
        RefreshToken GenerateRefreshToken();
    }
}
