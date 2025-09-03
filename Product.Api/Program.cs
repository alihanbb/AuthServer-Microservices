using System.Text;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    options.AddPolicy("UserOnly", policy => policy.RequireRole("User")); // "Order" yerine "User"

    options.AddPolicy("StaffOnly", policy => policy.RequireRole("Staff"));
    
    options.AddPolicy("ManagerOnly", policy => policy.RequireRole("Manager")); // Manager rolü ekleyebilirsiniz
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();


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

