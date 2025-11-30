using Customer.Application.Behavior;
using Customer.Application.Customer.Services;
using Customer.Infrastructure.Customer;
using Customer.Infrastructure.EventConsumers;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using SharedLibrary.Messaging;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CustomerDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("CustomerDbConnection")));


builder.Services.AddScoped<ICustomerService, CustomerServices>();
builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddAutoMapper(typeof(Customer.Infrastructure.AssemblyReference).Assembly);
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Customer.Application.AssemblyReference).Assembly));

builder.Services.AddValidatorsFromAssembly(typeof(Customer.Application.AssemblyReference).Assembly);
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

// MassTransit RabbitMQ Integration with Order Event Consumers
builder.Services.AddMassTransitWithRabbitMq("CustomerService", x =>
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
        options.AddAudiences("customer-api");

        // Configure introspection endpoint
        options.UseIntrospection()
               .SetClientId("customer-api")
               .SetClientSecret("customer-secret-key-2024");

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
    options.AddPolicy("CustomerRead", policy => 
        policy.RequireClaim("scope", "customer.read"));
    
    options.AddPolicy("CustomerWrite", policy => 
        policy.RequireClaim("scope", "customer.write"));
    
    // Role-based policies (for backward compatibility)
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserOnly", policy => policy.RequireRole("User")); 
    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager"));
});

var app = builder.Build();

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
