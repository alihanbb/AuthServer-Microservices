using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Order.Application.Commands.CreateOrder;
using Order.Application.Queries.GetOrder;
using Order.Infrastructure.Consumers;
using Order.Infrastructure.Persistence;
using Scalar.AspNetCore;
using SharedBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<OrderDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderConnection")));

builder.Services.AddScoped<IOrderDbContext>(sp => sp.GetRequiredService<OrderDbContext>());

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Order.Application.Behavior.ValidationBehavior<,>));

// MassTransit with Azure Service Bus
builder.Services.AddSharedBusWithAzureServiceBus(builder.Configuration, "OrderService", x =>
{
    x.AddConsumer<OrderCompletedEventConsumer>();
    x.AddConsumer<OrderFailedEventConsumer>();
});

// Add Saga
builder.Services.AddOrderSaga(builder.Configuration);

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("OrderConnection")!,
        name: "order-database",
        tags: new[] { "database", "postgresql" })
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "api" });

builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(30);
    setup.AddHealthCheckEndpoint("Order API", "/health");
}).AddInMemoryStorage();

// OpenIddict Validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("http://authserver.api:8080/");
        options.AddAudiences("order-api");
        options.UseIntrospection()
               .SetClientId("order-api")
               .SetClientSecret("order-secret-key-2024");
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("OrderRead", policy => policy.RequireClaim("scope", "order.read"));
    options.AddPolicy("OrderWrite", policy => policy.RequireClaim("scope", "order.write"));
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
var ordersGroup = app.MapGroup("/api/orders")
    .WithTags("Orders")
    .WithOpenApi();

// POST /api/orders - Create Order
ordersGroup.MapPost("/", async (CreateOrderCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Created($"/api/orders/{result.OrderId}", result);
})
.RequireAuthorization("OrderWrite")
.WithName("CreateOrder")
.WithSummary("Create a new order")
.WithDescription("Creates a new order and initiates the saga orchestration process");

// GET /api/orders/{id} - Get Order
ordersGroup.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var query = new GetOrderQuery { Id = id };
    var result = await mediator.Send(query);
    return result is not null ? Results.Ok(result) : Results.NotFound();
})
.RequireAuthorization("OrderRead")
.WithName("GetOrder")
.WithSummary("Get order by ID")
.WithDescription("Retrieves order details by order ID");

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
    var context = scope.ServiceProvider.GetRequiredService<OrderDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
