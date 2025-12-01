using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Customer.Application.Commands.CreateCustomer;
using Customer.Infrastructure.Consumers;
using Customer.Infrastructure.Customer;
using Scalar.AspNetCore;
using SharedBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CustomerConnection")));

builder.Services.AddScoped<ICustomerDbContext>(sp => sp.GetRequiredService<CustomerDbContext>());

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateCustomerCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateCustomerCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Customer.Application.Behavior.ValidationBehavior<,>));

// MassTransit with Azure Service Bus
builder.Services.AddSharedBusWithAzureServiceBus(builder.Configuration, "CustomerService", x =>
{
    x.AddConsumer<ValidateCustomerConsumer>();
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("CustomerConnection")!,
        name: "customer-database",
        tags: new[] { "database", "postgresql" })
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "api" });

builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(30);
    setup.AddHealthCheckEndpoint("Customer API", "/health");
}).AddInMemoryStorage();

// OpenIddict Validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("http://authserver.api:8080/");
        options.AddAudiences("customer-api");
        options.UseIntrospection()
               .SetClientId("customer-api")
               .SetClientSecret("customer-secret-key-2024");
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CustomerRead", policy => policy.RequireClaim("scope", "customer.read"));
    options.AddPolicy("CustomerWrite", policy => policy.RequireClaim("scope", "customer.write"));
});

// OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure pipeline
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseAuthentication();
app.UseAuthorization();

// Minimal API Endpoints
var customersGroup = app.MapGroup("/api/customers")
    .WithTags("Customers")
    .WithOpenApi();

// POST /api/customers - Create Customer
customersGroup.MapPost("/", async (CreateCustomerCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Created($"/api/customers/{result.CustomerId}", result);
})
.RequireAuthorization("CustomerWrite")
.WithName("CreateCustomer")
.WithSummary("Create a new customer")
.WithDescription("Creates a new customer in the system");

// Health Check Endpoints
app.MapHealthChecks("/health", new HealthCheckOptions
{
    Predicate = _ => true,
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapHealthChecksUI(options =>
{
    options.UIPath = "/health-ui";
    options.ApiPath = "/health-api";
});

// Apply migrations on startup (for development)
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<CustomerDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
