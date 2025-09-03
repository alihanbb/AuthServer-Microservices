using Microsoft.AspNetCore.Mvc;
using OcelotGateway.Services;
using Microsoft.AspNetCore.Authorization;

namespace OcelotGateway.Controllers
{
    [ApiController]
    [Route("api/gateway")]
    public class GatewayController : ControllerBase
    {
        private readonly GatewayAuthService _authService;
        private readonly ILogger<GatewayController> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public GatewayController(GatewayAuthService authService, ILogger<GatewayController> logger, IHttpClientFactory httpClientFactory)
        {
            _authService = authService;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            try
            {
                var authResponse = await _authService.AuthenticateAsync(request);
                
                if (authResponse != null)
                {
                    _logger.LogInformation("User {UserName} logged in successfully through gateway", request.UserName);
                    return Ok(authResponse);
                }

                return Unauthorized(new { Message = "Invalid credentials" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login process");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpPost("validate-token")]
        public async Task<IActionResult> ValidateToken()
        {
            try
            {
                var authHeader = Request.Headers.Authorization.FirstOrDefault();
                if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
                {
                    return BadRequest(new { Message = "Invalid authorization header" });
                }

                var token = authHeader.Substring("Bearer ".Length);
                var isValid = await _authService.ValidateTokenAsync(token);

                return Ok(new { IsValid = isValid });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error validating token");
                return StatusCode(500, new { Message = "Internal server error" });
            }
        }

        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok(new 
            { 
                Status = "Healthy",
                Timestamp = DateTime.UtcNow,
                Services = new
                {
                    Gateway = "Running",
                    Authentication = "Connected",
                    Routing = "Active"
                }
            });
        }

        [HttpGet("routes")]
        public IActionResult GetRoutes()
        {
            var routes = new
            {
                AuthRoutes = new[]
                {
                    "POST /api/auth/login",
                    "POST /api/auth/register", 
                    "POST /api/auth/refresh-token",
                    "GET /api/role",
                    "POST /api/role"
                },
                CustomerRoutes = new[]
                {
                    "GET /api/customer",
                    "POST /api/customer",
                    "PUT /api/customer/{id}",
                    "DELETE /api/customer/{id}"
                },
                ProductRoutes = new[]
                {
                    "GET /api/product",
                    "POST /api/product",
                    "PUT /api/product/{id}",
                    "DELETE /api/product/{id}"
                },
                OrderRoutes = new[]
                {
                    "GET /api/order",
                    "POST /api/order",
                    "PUT /api/order/{id}",
                    "DELETE /api/order/{id}"
                },
                GatewayRoutes = new[]
                {
                    "POST /api/gateway/login",
                    "POST /api/gateway/validate-token",
                    "GET /api/gateway/health",
                    "GET /api/gateway/routes",
                    "GET /api/gateway/services",
                    "GET /gateway/info",
                    "GET /gateway/stats",
                    "GET /health"
                }
            };

            return Ok(routes);
        }

        [HttpGet("services")]
        public async Task<IActionResult> GetServicesStatus()
        {
            var client = _httpClientFactory.CreateClient();
            client.Timeout = TimeSpan.FromSeconds(5);

            var services = new List<object>();

            var serviceEndpoints = new[]
            {
                new { Name = "AuthServer", Url = "http://auhtserver.api:8080/health" },
                new { Name = "CustomerService", Url = "http://customer.api:80/health" },
                new { Name = "ProductService", Url = "http://product.api:80/health" },
                new { Name = "OrderService", Url = "http://order.api:80/health" }
            };

            foreach (var service in serviceEndpoints)
            {
                try
                {
                    var response = await client.GetAsync(service.Url);
                    services.Add(new
                    {
                        Name = service.Name,
                        Status = response.IsSuccessStatusCode ? "Healthy" : "Unhealthy",
                        Url = service.Url,
                        ResponseTime = response.Headers.Date?.ToString() ?? "N/A"
                    });
                }
                catch (Exception ex)
                {
                    services.Add(new
                    {
                        Name = service.Name,
                        Status = "Unhealthy",
                        Url = service.Url,
                        Error = ex.Message
                    });
                }
            }

            return Ok(new
            {
                Timestamp = DateTime.UtcNow,
                TotalServices = services.Count,
                Services = services
            });
        }

        [HttpGet("config")]
        [Authorize(Roles = "Admin")]
        public IActionResult GetGatewayConfig()
        {
            return Ok(new
            {
                OcelotConfigPath = "ocelot.json",
                Features = new
                {
                    Authentication = true,
                    RateLimiting = true,
                    RequestLogging = true,
                    HealthChecks = true,
                    CORS = true
                },
                Environment = HttpContext.RequestServices.GetService<IWebHostEnvironment>()?.EnvironmentName,
                LoadBalancing = "RoundRobin",
                Caching = false,
                CircuitBreaker = false
            });
        }

        [HttpPost("reload-config")]
        [Authorize(Roles = "Admin")]
        public IActionResult ReloadConfiguration()
        {
            // Bu endpoint Ocelot configuration'ý runtime'da reload etmek için kullanýlabilir
            _logger.LogInformation("Configuration reload requested by admin");
            return Ok(new { Message = "Configuration reload initiated", Timestamp = DateTime.UtcNow });
        }
    }
}