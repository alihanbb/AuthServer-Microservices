using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using OcelotGateway.Middlewares;
using OcelotGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Ocelot configuration - Environment specific
var ocelotConfigFile = builder.Environment.IsDevelopment() 
    ? "ocelot.Development.json" 
    : "ocelot.json";

builder.Configuration.AddJsonFile(ocelotConfigFile, optional: false, reloadOnChange: true);

// Controllers
builder.Services.AddControllers();

// HttpClient for services
builder.Services.AddHttpClient<GatewayAuthService>();

// JWT Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer("Bearer", options =>
    {
        options.RequireHttpsMetadata = false; // Development için
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "default-key"))
        };
    });

// CORS Policy
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Health Checks - Environment specific URLs
var healthCheckUrls = builder.Environment.IsDevelopment() 
    ? new[]
    {
        new { Name = "AuthServer", Url = "https://localhost:7001/health" },
        new { Name = "CustomerService", Url = "https://localhost:7002/health" },
        new { Name = "ProductService", Url = "https://localhost:7003/health" },
        new { Name = "OrderService", Url = "https://localhost:7004/health" }
    }
    : new[]
    {
        new { Name = "AuthServer", Url = "http://auhtserver.api:8080/health" },
        new { Name = "CustomerService", Url = "http://customer.api:80/health" },
        new { Name = "ProductService", Url = "http://product.api:80/health" },
        new { Name = "OrderService", Url = "http://order.api:80/health" }
    };

var healthChecks = builder.Services.AddHealthChecks()
    .AddCheck("Gateway Health", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("Gateway is running"));

foreach (var healthCheckUrl in healthCheckUrls)
{
    healthChecks.AddUrlGroup(new Uri(healthCheckUrl.Url), healthCheckUrl.Name);
}

// Ocelot
builder.Services.AddOcelot();

// Services
builder.Services.AddScoped<GatewayAuthService>();

// Logging
builder.Services.AddLogging(logging =>
{
    logging.AddConsole();
    logging.AddDebug();
});

// OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure pipeline
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseMiddleware<RateLimitingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

// Map controllers for custom endpoints
app.MapControllers();

// Health check endpoint with detailed info
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var response = new
        {
            Status = report.Status.ToString(),
            Checks = report.Entries.Select(x => new
            {
                Name = x.Key,
                Status = x.Value.Status.ToString(),
                Description = x.Value.Description,
                Duration = x.Value.Duration
            }),
            TotalDuration = report.TotalDuration
        };
        await context.Response.WriteAsync(System.Text.Json.JsonSerializer.Serialize(response));
    }
});

// Enhanced Gateway info endpoint - Environment specific
var serviceUrls = builder.Environment.IsDevelopment()
    ? new 
    {
        AuthServer = "https://localhost:7001",
        CustomerService = "https://localhost:7002",
        ProductService = "https://localhost:7003", 
        OrderService = "https://localhost:7004"
    }
    : new 
    {
        AuthServer = "http://auhtserver.api:8080",
        CustomerService = "http://customer.api:80",
        ProductService = "http://product.api:80", 
        OrderService = "http://order.api:80"
    };

app.MapGet("/gateway/info", () => new
{
    Name = "Ocelot API Gateway",
    Version = "1.0.0",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTime.UtcNow,
    Services = serviceUrls,
    Features = new[]
    {
        "JWT Authentication",
        "Rate Limiting", 
        "Request Logging",
        "Health Monitoring",
        "Route Management",
        "CORS Support"
    }
});

// Gateway statistics endpoint
app.MapGet("/gateway/stats", () => new
{
    TotalRequests = "Implemented via middleware",
    ActiveConnections = "Monitored",
    AverageResponseTime = "Calculated",
    ErrorRate = "Tracked"
});

// Ocelot Gateway - Bu en son olmalý
await app.UseOcelot();

app.Run();
