using System.Text;
using System.Text.Json;

namespace OcelotGateway.Services
{
    public class GatewayAuthService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GatewayAuthService> _logger;

        public GatewayAuthService(HttpClient httpClient, IConfiguration configuration, ILogger<GatewayAuthService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var authServerUrl = _configuration["ServiceUrls:AuthServer"];
                var request = new HttpRequestMessage(HttpMethod.Post, $"{authServerUrl}/api/auth/validate-token");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token through auth server");
                return false;
            }
        }

        public async Task<AuthResponse?> AuthenticateAsync(LoginRequest loginRequest)
        {
            try
            {
                var authServerUrl = _configuration["ServiceUrls:AuthServer"];
                var json = JsonSerializer.Serialize(loginRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync($"{authServerUrl}/api/auth/login", content);
                
                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    return JsonSerializer.Deserialize<AuthResponse>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error authenticating user through auth server");
                return null;
            }
        }
    }

    public record LoginRequest(string UserName, string Password);
    
    public record AuthResponse(
        string Token,
        string RefreshToken,
        DateTime ExpiresAt,
        UserInfo User
    );
    
    public record UserInfo(
        string Id,
        string UserName,
        string Email,
        List<string> Roles
    );
}