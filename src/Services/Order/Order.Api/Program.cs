using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Order.Application.Order.Services;
using Order.Infrastructures.Order;
using Order.Application.Behavior;
using Scalar.AspNetCore;
using SharedLibrary.Messaging;
using Order.Infrastructures.EventConsumers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OrderDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("OrderConnection"));
});


builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddAutoMapper(typeof(Order.Infrastructures.AssemblyReference).Assembly);
builder.Services.AddMediatR(cfg =>
cfg.RegisterServicesFromAssembly(typeof(Order.Application.AssemblyReference).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Order.Application.AssemblyReference).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// MassTransit RabbitMQ Integration with Event Consumers
builder.Services.AddMassTransitWithRabbitMq("OrderService", x =>
{
    // Add event consumers for Customer and Product events
    x.AddConsumer<CustomerCreatedEventConsumer>();
    x.AddConsumer<CustomerDeletedEventConsumer>();
    x.AddConsumer<ProductPriceChangedEventConsumer>();
    x.AddConsumer<ProductStockUpdatedEventConsumer>();
    x.AddConsumer<ProductDeletedEventConsumer>();
});

// OpenIddict Validation Configuration
builder.Services.AddOpenIddict()
    .AddValidation(options =>
    {
        // Set the issuer (AuthServer URL)
        options.SetIssuer("http://auhtserver.api:8080/");
        options.AddAudiences("order-api");

        // Configure introspection endpoint
        options.UseIntrospection()
               .SetClientId("order-api")
               .SetClientSecret("order-secret-key-2024");

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
    options.AddPolicy("OrderRead", policy => 
        policy.RequireClaim("scope", "order.read"));
    
    options.AddPolicy("OrderWrite", policy => 
        policy.RequireClaim("scope", "order.write"));
    
    // Role-based policies (for backward compatibility)
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User"));
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddControllers();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
