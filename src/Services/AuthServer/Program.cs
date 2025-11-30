using Authserver.Application.Interfaces;
using Authserver.Infrıastructure.Persistence.Context;
using Authserver.Infrıastructure.Security;
using AuthServerDomain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Abstractions;
using static OpenIddict.Abstractions.OpenIddictConstants;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Database Configuration
builder.Services.AddDbContext<AuthDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("AuthServerDb"));
    options.UseOpenIddict();
});

// Redis Cache Configuration
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisCache");
    options.InstanceName = "AuthServer:";
});

// ASP.NET Core Identity
builder.Services.AddIdentity<AppUser, IdentityRole<Guid>>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<AuthDbContext>()
.AddDefaultTokenProviders();

// OpenIddict Configuration
builder.Services.AddOpenIddict()
    .AddCore(options =>
    {
        options.UseEntityFrameworkCore()
               .UseDbContext<AuthDbContext>();
    })
    .AddServer(options =>
    {
        // OAuth2/OIDC endpoints
        options.SetTokenEndpointUris("/connect/token")
               .SetAuthorizationEndpointUris("/connect/authorize")
               .SetUserinfoEndpointUris("/connect/userinfo")
               .SetIntrospectionEndpointUris("/connect/introspect")
               .SetRevocationEndpointUris("/connect/revoke");

        // OAuth2 Flows
        options.AllowClientCredentialsFlow()      // Mikroservisler arası
               .AllowPasswordFlow()               // Kullanıcı login
               .AllowAuthorizationCodeFlow()      // Web apps
               .AllowRefreshTokenFlow();          // Refresh tokens

        // Token configuration
        options.SetAccessTokenLifetime(TimeSpan.FromHours(1))
               .SetRefreshTokenLifetime(TimeSpan.FromDays(7));

        // Encryption & Signing - Development
        options.AddDevelopmentEncryptionCertificate()
               .AddDevelopmentSigningCertificate();
        // Production'da gerçek sertifikalar kullanılmalı:
        // options.AddEncryptionCertificate(certificate)
        //        .AddSigningCertificate(certificate);

        // ASP.NET Core integration
        options.UseAspNetCore()
               .EnableTokenEndpointPassthrough()
               .EnableAuthorizationEndpointPassthrough()
               .EnableUserinfoEndpointPassthrough();
    })
    .AddValidation(options =>
    {
        options.UseLocalServer();
        options.UseAspNetCore();
    });



// Health Checks
builder.Services.AddHealthChecks();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Seed OpenIddict clients
await SeedOpenIddictClients(app.Services);

// Configure the HTTP request pipeline

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// OpenIddict Client Seeding
static async Task SeedOpenIddictClients(IServiceProvider services)
{
    using var scope = services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await context.Database.EnsureCreatedAsync();

    var manager = scope.ServiceProvider.GetRequiredService<IOpenIddictApplicationManager>();

    // Customer API Client
    if (await manager.FindByClientIdAsync("customer-api") == null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "customer-api",
            ClientSecret = "customer-secret-key-2024",
            DisplayName = "Customer Microservice",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Prefixes.Scope + "customer.read",
                Permissions.Prefixes.Scope + "customer.write"
            }
        });
    }

    // Product API Client
    if (await manager.FindByClientIdAsync("product-api") == null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "product-api",
            ClientSecret = "product-secret-key-2024",
            DisplayName = "Product Microservice",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Prefixes.Scope + "product.read",
                Permissions.Prefixes.Scope + "product.write"
            }
        });
    }

    // Order API Client
    if (await manager.FindByClientIdAsync("order-api") == null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "order-api",
            ClientSecret = "order-secret-key-2024",
            DisplayName = "Order Microservice",
            Permissions =
            {
                Permissions.Endpoints.Token,
                Permissions.Endpoints.Introspection,
                Permissions.GrantTypes.ClientCredentials,
                Permissions.Prefixes.Scope + "order.read",
                Permissions.Prefixes.Scope + "order.write"
            }
        });
    }

    // Web Client (for future web/mobile apps)
    if (await manager.FindByClientIdAsync("web-client") == null)
    {
        await manager.CreateAsync(new OpenIddictApplicationDescriptor
        {
            ClientId = "web-client",
            DisplayName = "Web Application",
            RedirectUris = { new Uri("https://localhost:5001/signin-oidc") },
            PostLogoutRedirectUris = { new Uri("https://localhost:5001/signout-callback-oidc") },
            Permissions =
            {
                Permissions.Endpoints.Authorization,
                Permissions.Endpoints.Token,
                Permissions.GrantTypes.AuthorizationCode,
                Permissions.GrantTypes.RefreshToken,
                Permissions.ResponseTypes.Code,
                Permissions.Scopes.Email,
                Permissions.Scopes.Profile,
                Permissions.Scopes.Roles,
                Permissions.Prefixes.Scope + "customer.read",
                Permissions.Prefixes.Scope + "product.read",
                Permissions.Prefixes.Scope + "order.read"
            },
            Requirements =
            {
                Requirements.Features.ProofKeyForCodeExchange
            }
        });
    }

    // Seed a test user
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
    if (await userManager.FindByNameAsync("testuser") == null)
    {
        var testUser = new AppUser
        {
            UserName = "testuser",
            Email = "test@authserver.com",
            EmailConfirmed = true,
            FirstName = "Test",
            LastName = "User"
        };

        await userManager.CreateAsync(testUser, "Test123!");
    }
}
