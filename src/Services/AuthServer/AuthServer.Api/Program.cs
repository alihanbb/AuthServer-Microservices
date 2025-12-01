using AuthServer.Api.Extensions;
using AuthServer.Domain.Entities;
using AuthServer.Infrastructure.Persistence;
using AuthServer.Redis.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Controllers & Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add all services using extension methods
builder.Services.AddDatabaseServices(builder.Configuration);
builder.Services.AddRedisServices(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.AddApplicationServices();
builder.Services.AddOpenIddictServices(builder.Configuration);
builder.Services.AddHealthCheckServices(builder.Configuration);

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();

// Static Files for Admin UI
app.UseStaticFiles();

// Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

// Database Migration & Seed Data
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    
    // Apply pending migrations
    await context.Database.MigrateAsync();

    // Seed data
    await AuthServer.Infrastructure.Data.DataSeeder.SeedAsync(scope.ServiceProvider);
}

app.Run();
