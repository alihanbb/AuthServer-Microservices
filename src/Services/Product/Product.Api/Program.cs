using FluentValidation;
using HealthChecks.UI.Client;
using MediatR;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Product.Application.Commands.CreateProduct;
using Product.Application.Queries.GetProduct;
using Product.Infrastructure.Consumers;
using Product.Infrastructure.Products;
using Scalar.AspNetCore;
using SharedBus.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ProductConnection")));

builder.Services.AddScoped<IProductDbContext>(sp => sp.GetRequiredService<ProductDbContext>());

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateProductCommand).Assembly));

// FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommand).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(Product.Application.Behavior.ValidationBehavior<,>));

// MassTransit with Azure Service Bus
builder.Services.AddSharedBusWithAzureServiceBus(builder.Configuration, "ProductService", x =>
{
    x.AddConsumer<UpdateProductStockConsumer>();
});

// Health Checks
builder.Services.AddHealthChecks()
    .AddNpgSql(
        builder.Configuration.GetConnectionString("ProductConnection")!,
        name: "product-database",
        tags: new[] { "database", "postgresql" })
    .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: new[] { "api" });

builder.Services.AddHealthChecksUI(setup =>
{
    setup.SetEvaluationTimeInSeconds(30);
    setup.AddHealthCheckEndpoint("Product API", "/health");
}).AddInMemoryStorage();

// OpenIddict Validation
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        options.SetIssuer("http://authserver.api:8080/");
        options.AddAudiences("product-api");
        options.UseIntrospection()
               .SetClientId("product-api")
               .SetClientSecret("product-secret-key-2024");
        options.UseAspNetCore();
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ProductRead", policy => policy.RequireClaim("scope", "product.read"));
    options.AddPolicy("ProductWrite", policy => policy.RequireClaim("scope", "product.write"));
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
var productsGroup = app.MapGroup("/api/products")
    .WithTags("Products")
    .WithOpenApi();

// POST /api/products - Create Product
productsGroup.MapPost("/", async (CreateProductCommand command, IMediator mediator) =>
{
    var result = await mediator.Send(command);
    return Results.Created($"/api/products/{result.ProductId}", result);
})
.RequireAuthorization("ProductWrite")
.WithName("CreateProduct")
.WithSummary("Create a new product")
.WithDescription("Creates a new product in the catalog");

// GET /api/products/{id} - Get Product
productsGroup.MapGet("/{id:guid}", async (Guid id, IMediator mediator) =>
{
    var query = new GetProductQuery { Id = id };
    var result = await mediator.Send(query);
    return result is not null ? Results.Ok(result) : Results.NotFound();
})
.RequireAuthorization("ProductRead")
.WithName("GetProduct")
.WithSummary("Get product by ID")
.WithDescription("Retrieves product details by product ID");

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
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    await context.Database.MigrateAsync();
}

app.Run();
