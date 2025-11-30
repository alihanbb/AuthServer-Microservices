using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Product.Domain.Entities;

namespace Product.Infrastructure.Products
{
    public class ProductBaseDbContext : DbContext
    {
        public ProductBaseDbContext(DbContextOptions<ProductBaseDbContext> options) : base(options) { }
       
        public DbSet<ProductBase> Products { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
