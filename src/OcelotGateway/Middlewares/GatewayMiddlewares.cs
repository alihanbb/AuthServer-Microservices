using System.Net;
using System.Text.Json;

namespace OcelotGateway.Middlewares
{
    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;

        public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred in Gateway");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Message = "An error occurred while processing your request through the gateway.",
                Details = exception.Message
            };

            var jsonResponse = JsonSerializer.Serialize(response);
            await context.Response.WriteAsync(jsonResponse);
        }
    }

    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var startTime = DateTime.UtcNow;
            
            _logger.LogInformation("Gateway Request: {Method} {Path} from {RemoteIP}", 
                context.Request.Method, 
                context.Request.Path, 
                context.Connection.RemoteIpAddress);

            await _next(context);

            var duration = DateTime.UtcNow - startTime;
            
            _logger.LogInformation("Gateway Response: {StatusCode} in {Duration}ms", 
                context.Response.StatusCode, 
                duration.TotalMilliseconds);
        }
    }

    public class RateLimitingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RateLimitingMiddleware> _logger;
        private static readonly Dictionary<string, List<DateTime>> _requestLog = new();
        private static readonly object _lock = new object();
        private const int MaxRequests = 100; // 1 dakikada maksimum 100 istek
        private const int TimeWindowMinutes = 1;

        public RateLimitingMiddleware(RequestDelegate next, ILogger<RateLimitingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var clientId = GetClientIdentifier(context);
            
            if (IsRateLimited(clientId))
            {
                context.Response.StatusCode = 429; // Too Many Requests
                context.Response.ContentType = "application/json";
                
                var response = new
                {
                    StatusCode = 429,
                    Message = "Rate limit exceeded. Please try again later.",
                    RetryAfter = TimeWindowMinutes * 60
                };

                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }

            LogRequest(clientId);
            await _next(context);
        }

        private static string GetClientIdentifier(HttpContext context)
        {
            // IP adresini client identifier olarak kullan
            return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        }

        private static bool IsRateLimited(string clientId)
        {
            lock (_lock)
            {
                if (!_requestLog.ContainsKey(clientId))
                {
                    _requestLog[clientId] = new List<DateTime>();
                }

                var requests = _requestLog[clientId];
                var cutoff = DateTime.UtcNow.AddMinutes(-TimeWindowMinutes);
                
                // Eski istekleri temizle
                requests.RemoveAll(time => time < cutoff);
                
                return requests.Count >= MaxRequests;
            }
        }

        private static void LogRequest(string clientId)
        {
            lock (_lock)
            {
                if (!_requestLog.ContainsKey(clientId))
                {
                    _requestLog[clientId] = new List<DateTime>();
                }

                _requestLog[clientId].Add(DateTime.UtcNow);
            }
        }
    }
}