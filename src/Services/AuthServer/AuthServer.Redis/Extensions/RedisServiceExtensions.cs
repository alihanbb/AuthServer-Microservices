using AuthServer.Redis.Interfaces;
using AuthServer.Redis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AuthServer.Redis.Extensions;

public static class RedisServiceExtensions
{
    public static IServiceCollection AddRedisServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Redis Distributed Cache
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = configuration.GetConnectionString("RedisCache");
            options.InstanceName = configuration["Redis:InstanceName"] ?? "AuthServer:";
        });

        // Token Revocation Service
        services.AddScoped<ITokenRevocationService, TokenRevocationService>();

        return services;
    }
}
