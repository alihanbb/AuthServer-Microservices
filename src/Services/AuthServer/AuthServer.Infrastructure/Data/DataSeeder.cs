using AuthServer.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using OpenIddict.Abstractions;

namespace AuthServer.Infrastructure.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
        var applicationManager = serviceProvider.GetRequiredService<IOpenIddictApplicationManager>();
        var scopeManager = serviceProvider.GetRequiredService<IOpenIddictScopeManager>();

        // Seed Roles
        await SeedRolesAsync(roleManager);

        // Seed Users
        await SeedUsersAsync(userManager, roleManager);

        // Seed OpenIddict Scopes
        await SeedScopesAsync(scopeManager);

        // Seed OpenIddict Applications
        await SeedApplicationsAsync(applicationManager);
    }

    private static async Task SeedRolesAsync(RoleManager<AppRole> roleManager)
    {
        var roles = new[]
        {
            new AppRole { Name = "SuperAdmin", Description = "Super Administrator with full system access" },
            new AppRole { Name = "Admin", Description = "Administrator with elevated privileges" },
            new AppRole { Name = "Manager", Description = "Manager with moderate privileges" },
            new AppRole { Name = "User", Description = "Standard user with basic privileges" }
        };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role.Name!))
            {
                await roleManager.CreateAsync(role);
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<AppUser> userManager, RoleManager<AppRole> roleManager)
    {
        var users = new[]
        {
            new
            {
                User = new AppUser
                {
                    UserName = "superadmin",
                    Email = "superadmin@authserver.com",
                    EmailConfirmed = true,
                    FirstName = "Super",
                    LastName = "Admin"
                },
                Password = "Password123!",
                Roles = new[] { "SuperAdmin", "Admin" }
            },
            new
            {
                User = new AppUser
                {
                    UserName = "admin",
                    Email = "admin@authserver.com",
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User"
                },
                Password = "Password123!",
                Roles = new[] { "Admin" }
            },
            new
            {
                User = new AppUser
                {
                    UserName = "manager",
                    Email = "manager@authserver.com",
                    EmailConfirmed = true,
                    FirstName = "Manager",
                    LastName = "User"
                },
                Password = "Password123!",
                Roles = new[] { "Manager" }
            },
            new
            {
                User = new AppUser
                {
                    UserName = "user",
                    Email = "user@authserver.com",
                    EmailConfirmed = true,
                    FirstName = "Regular",
                    LastName = "User"
                },
                Password = "Password123!",
                Roles = new[] { "User" }
            },
            new
            {
                User = new AppUser
                {
                    UserName = "john.doe",
                    Email = "john.doe@authserver.com",
                    EmailConfirmed = true,
                    FirstName = "John",
                    LastName = "Doe"
                },
                Password = "Password123!",
                Roles = new[] { "User" }
            },
            new
            {
                User = new AppUser
                {
                    UserName = "jane.smith",
                    Email = "jane.smith@authserver.com",
                    EmailConfirmed = true,
                    FirstName = "Jane",
                    LastName = "Smith"
                },
                Password = "Password123!",
                Roles = new[] { "Manager" }
            }
        };

        foreach (var userData in users)
        {
            var existingUser = await userManager.FindByNameAsync(userData.User.UserName!);
            if (existingUser == null)
            {
                var result = await userManager.CreateAsync(userData.User, userData.Password);
                if (result.Succeeded)
                {
                    foreach (var role in userData.Roles)
                    {
                        if (await roleManager.RoleExistsAsync(role))
                        {
                            await userManager.AddToRoleAsync(userData.User, role);
                        }
                    }
                }
            }
        }
    }

    private static async Task SeedScopesAsync(IOpenIddictScopeManager scopeManager)
    {
        var scopes = new[]
        {
            new { Name = "api", DisplayName = "API Access", Description = "Access to the API" },
            new { Name = "openid", DisplayName = "OpenID", Description = "OpenID Connect scope" },
            new { Name = "profile", DisplayName = "Profile", Description = "User profile information" },
            new { Name = "email", DisplayName = "Email", Description = "User email address" },
            new { Name = "roles", DisplayName = "Roles", Description = "User roles" },
            new { Name = "offline_access", DisplayName = "Offline Access", Description = "Refresh token support" }
        };

        foreach (var scopeData in scopes)
        {
            var existingScope = await scopeManager.FindByNameAsync(scopeData.Name);
            if (existingScope == null)
            {
                await scopeManager.CreateAsync(new OpenIddictScopeDescriptor
                {
                    Name = scopeData.Name,
                    DisplayName = scopeData.DisplayName,
                    Description = scopeData.Description,
                    Resources = { "auth_server_api" }
                });
            }
        }
    }

    private static async Task SeedApplicationsAsync(IOpenIddictApplicationManager applicationManager)
    {
        // Web Application (Authorization Code Flow)
        if (await applicationManager.FindByClientIdAsync("web_app") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "web_app",
                ClientSecret = "web_app_secret",
                DisplayName = "Web Application",
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                Type = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.Endpoints.Logout,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    "scp:api",
                    "scp:offline_access"
                },
                RedirectUris = { new Uri("https://localhost:5001/signin-oidc") },
                PostLogoutRedirectUris = { new Uri("https://localhost:5001/signout-callback-oidc") }
            });
        }

        // Mobile/SPA Application (PKCE Flow)
        if (await applicationManager.FindByClientIdAsync("mobile_app") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "mobile_app",
                DisplayName = "Mobile Application",
                ConsentType = OpenIddictConstants.ConsentTypes.Explicit,
                Type = OpenIddictConstants.ClientTypes.Public,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Authorization,
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.AuthorizationCode,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.ResponseTypes.Code,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    "scp:api",
                    "scp:offline_access"
                },
                RedirectUris = { new Uri("myapp://callback") },
                Requirements = { OpenIddictConstants.Requirements.Features.ProofKeyForCodeExchange }
            });
        }

        // Service/Console Application (Client Credentials Flow)
        if (await applicationManager.FindByClientIdAsync("service_app") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "service_app",
                ClientSecret = "service_app_secret",
                DisplayName = "Service Application",
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                Type = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.ClientCredentials,
                    "scp:api"
                }
            });
        }

        // Postman/Testing Client (Password Flow - Development Only)
        if (await applicationManager.FindByClientIdAsync("postman") == null)
        {
            await applicationManager.CreateAsync(new OpenIddictApplicationDescriptor
            {
                ClientId = "postman",
                ClientSecret = "postman_secret",
                DisplayName = "Postman Testing Client",
                ConsentType = OpenIddictConstants.ConsentTypes.Implicit,
                Type = OpenIddictConstants.ClientTypes.Confidential,
                Permissions =
                {
                    OpenIddictConstants.Permissions.Endpoints.Token,
                    OpenIddictConstants.Permissions.GrantTypes.Password,
                    OpenIddictConstants.Permissions.GrantTypes.RefreshToken,
                    OpenIddictConstants.Permissions.Scopes.Email,
                    OpenIddictConstants.Permissions.Scopes.Profile,
                    OpenIddictConstants.Permissions.Scopes.Roles,
                    "scp:api",
                    "scp:offline_access"
                }
            });
        }
    }
}
