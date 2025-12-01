using AuthServer.Redis.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AuthServer.Redis.Services;

public class TokenRevocationService : ITokenRevocationService
{
    private readonly IDistributedCache _cache;
    private readonly IConfiguration _configuration;
    private readonly ILogger<TokenRevocationService> _logger;
    private readonly string _keyPrefix;

    public TokenRevocationService(
        IDistributedCache cache,
        IConfiguration configuration,
        ILogger<TokenRevocationService> logger)
    {
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        _keyPrefix = configuration["Redis:TokenBlacklistKeyPrefix"] ?? "blacklist:token:";
    }

    public async Task RevokeTokenAsync(string token, TimeSpan expiration)
    {
        try
        {
            var key = GetBlacklistKey(token);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            };

            await _cache.SetStringAsync(key, DateTime.UtcNow.ToString("O"), options);
            _logger.LogInformation("Token revoked and added to blacklist: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to revoke token");
            throw;
        }
    }

    public async Task<bool> IsTokenRevokedAsync(string token)
    {
        try
        {
            var key = GetBlacklistKey(token);
            var value = await _cache.GetStringAsync(key);
            
            var isRevoked = !string.IsNullOrEmpty(value);
            
            if (isRevoked)
            {
                _logger.LogWarning("Blocked revoked token. Revoked at: {RevokedAt}", value);
            }
            
            return isRevoked;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if token is revoked");
            return false;
        }
    }

    public async Task RemoveTokenFromBlacklistAsync(string token)
    {
        try
        {
            var key = GetBlacklistKey(token);
            await _cache.RemoveAsync(key);
            _logger.LogInformation("Token removed from blacklist: {Key}", key);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove token from blacklist");
            throw;
        }
    }

    private string GetBlacklistKey(string token)
    {
        var tokenHash = Convert.ToBase64String(
            System.Security.Cryptography.SHA256.HashData(
                System.Text.Encoding.UTF8.GetBytes(token)
            )
        );
        return $"{_keyPrefix}{tokenHash}";
    }
}
