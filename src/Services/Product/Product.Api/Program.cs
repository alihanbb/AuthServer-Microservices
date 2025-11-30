using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Product.Application.Behavior;
using Product.Application.Services;
using Product.Infrastructure.Products;
using Product.Infrastructure.EventConsumers;
using Scalar.AspNetCore;
using SharedLibrary.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddDbContext<ProductBaseDbContext>(opt =>
{
    opt.UseNpgsql(builder.Configuration.GetConnectionString("ProductDbConnections"));
});

builder.Services.AddAutoMapper(typeof(Product.Infrastructure.AssemblyReference).Assembly);
builder.Services.AddMediatR(cfg => 
cfg.RegisterServicesFromAssembly(typeof(Product.Application.AssemblyReference).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Product.Application.AssemblyReference).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// MassTransit RabbitMQ Integration with Order Event Consumers
builder.Services.AddMassTransitWithRabbitMq("ProductService", x =>
{
    // Add order event consumers
    x.AddConsumer<OrderCreatedEventConsumer>();
    x.AddConsumer<OrderStatusChangedEventConsumer>();
    x.AddConsumer<OrderDeletedEventConsumer>();
});

// OpenIddict Validation Configuration
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Set the issuer (AuthServer URL)
        options.SetIssuer("http://auhtserver.api:8080/");
        options.AddAudiences("product-api");

        // Configure introspection endpoint
        options.UseIntrospection()
               .SetClientId("product-api")
               .SetClientSecret("product-secret-key-2024");

        options.UseAspNetCore();
    });

// Configure authentication to use OpenIddict
builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = OpenIddict.Validation.AspNetCore.OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options =>
{
    // Scope-based policies
    options.AddPolicy("ProductRead", policy => 
        policy.RequireClaim("scope", "product.read"));
    
    options.AddPolicy("ProductWrite", policy => 
        policy.RequireClaim("scope", "product.write"));
    
    // Role-based policies (for backward compatibility)
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager")); 
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
  
}
app.MapOpenApi();
app.MapScalarApiReference();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


app.Run();
