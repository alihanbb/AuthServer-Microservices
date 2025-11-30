using AuthServerDomain.Entities;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using System.Security.Claims;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace AuhtServer.Api.Controllers
{
    [ApiController]
    public class AuthorizationController : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IOpenIddictApplicationManager _applicationManager;

        public AuthorizationController(
            UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager,
            IOpenIddictApplicationManager applicationManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _applicationManager = applicationManager;
        }

        [HttpPost("/connect/token")]
        [Produces("application/json")]
        public async Task<IActionResult> Exchange()
        {
            var request = HttpContext.GetOpenIddictServerRequest() 
                ?? throw new InvalidOperationException("The OpenID Connect request cannot be retrieved.");

            // Client Credentials Flow (for microservices)
            if (request.IsClientCredentialsGrantType())
            {
                var application = await _applicationManager.FindByClientIdAsync(request.ClientId!)
                    ?? throw new InvalidOperationException("The application details cannot be found.");

                var identity = new ClaimsIdentity(
                    TokenValidationParameters.DefaultAuthenticationType,
                    Claims.Name,
                    Claims.Role);

                identity.AddClaim(Claims.Subject, await _applicationManager.GetClientIdAsync(application)!);
                identity.AddClaim(Claims.Name, await _applicationManager.GetDisplayNameAsync(application)!);

                // Add scopes
                identity.SetScopes(request.GetScopes());
                identity.SetDestinations(GetDestinations);

                var principal = new ClaimsPrincipal(identity);

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // Password Flow (for user authentication)
            if (request.IsPasswordGrantType())
            {
                var user = await _userManager.FindByNameAsync(request.Username!);
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                        }));
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password!, lockoutOnFailure: true);
                if (!result.Succeeded)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The username/password couple is invalid."
                        }));
                }

                var identity = new ClaimsIdentity(
                    TokenValidationParameters.DefaultAuthenticationType,
                    Claims.Name,
                    Claims.Role);

                identity.AddClaim(Claims.Subject, user.Id.ToString());
                identity.AddClaim(Claims.Name, user.UserName!);
                identity.AddClaim(Claims.Email, user.Email!);
                identity.AddClaim(Claims.GivenName, user.FirstName ?? string.Empty);
                identity.AddClaim(Claims.FamilyName, user.LastName ?? string.Empty);

                // Add user roles
                var roles = await _userManager.GetRolesAsync(user);
                foreach (var role in roles)
                {
                    identity.AddClaim(Claims.Role, role);
                }

                // Add scopes
                identity.SetScopes(request.GetScopes());
                identity.SetDestinations(GetDestinations);

                var principal = new ClaimsPrincipal(identity);

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            // Refresh Token Flow
            if (request.IsRefreshTokenGrantType())
            {
                var result = await HttpContext.AuthenticateAsync(OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);

                // Retrieve the user from the claims
                var userId = result.Principal?.GetClaim(Claims.Subject);
                if (userId == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is no longer valid."
                        }));
                }

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The refresh token is no longer valid."
                        }));
                }

                // Ensure the user is still allowed to sign in
                if (!await _signInManager.CanSignInAsync(user))
                {
                    return Forbid(
                        authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                        properties: new AuthenticationProperties(new Dictionary<string, string?>
                        {
                            [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidGrant,
                            [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The user is no longer allowed to sign in."
                        }));
                }

                var identity = new ClaimsIdentity(result.Principal!.Claims,
                    TokenValidationParameters.DefaultAuthenticationType,
                    Claims.Name,
                    Claims.Role);

                identity.SetDestinations(GetDestinations);

                var principal = new ClaimsPrincipal(identity);

                return SignIn(principal, OpenIddictServerAspNetCoreDefaults.AuthenticationScheme);
            }

            throw new InvalidOperationException("The specified grant type is not supported.");
        }

        [HttpGet("/connect/userinfo")]
        [HttpPost("/connect/userinfo")]
        [Produces("application/json")]
        public async Task<IActionResult> Userinfo()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(
                    authenticationSchemes: OpenIddictServerAspNetCoreDefaults.AuthenticationScheme,
                    properties: new AuthenticationProperties(new Dictionary<string, string?>
                    {
                        [OpenIddictServerAspNetCoreConstants.Properties.Error] = Errors.InvalidToken,
                        [OpenIddictServerAspNetCoreConstants.Properties.ErrorDescription] = "The specified access token is invalid."
                    }));
            }

            var claims = new Dictionary<string, object>(StringComparer.Ordinal)
            {
                [Claims.Subject] = user.Id.ToString(),
                [Claims.Name] = user.UserName!,
                [Claims.Email] = user.Email!
            };

            if (!string.IsNullOrEmpty(user.FirstName))
                claims[Claims.GivenName] = user.FirstName;

            if (!string.IsNullOrEmpty(user.LastName))
                claims[Claims.FamilyName] = user.LastName;

            // Add roles
            var roles = await _userManager.GetRolesAsync(user);
            if (roles.Any())
            {
                claims[Claims.Role] = roles.ToArray();
            }

            return Ok(claims);
        }

        private static IEnumerable<string> GetDestinations(Claim claim)
        {
            // Include claim in both access and identity tokens
            switch (claim.Type)
            {
                case Claims.Name:
                case Claims.Email:
                case Claims.Role:
                case Claims.Subject:
                    yield return Destinations.AccessToken;
                    yield return Destinations.IdentityToken;
                    break;

                default:
                    yield return Destinations.AccessToken;
                    break;
            }
        }
    }

    public static class TokenValidationParameters
    {
        public const string DefaultAuthenticationType = OpenIddictServerAspNetCoreDefaults.AuthenticationScheme;
    }
}
