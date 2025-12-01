namespace AuthServer.Redis.Interfaces;

public interface ITokenRevocationService
{
    Task RevokeTokenAsync(string token, TimeSpan expiration);
    Task<bool> IsTokenRevokedAsync(string token);
    Task RemoveTokenFromBlacklistAsync(string token);
}
