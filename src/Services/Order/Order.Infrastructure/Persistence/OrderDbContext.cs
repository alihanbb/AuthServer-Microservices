using Microsoft.EntityFrameworkCore;
using Order.Application.Commands.CreateOrder;
using Order.Domain.Order;
using SharedBus.Sagas;

namespace Order.Infrastructure.Persistence;

public class OrderDbContext : DbContext, IOrderDbContext
{
    public OrderDbContext(DbContextOptions<OrderDbContext> options) : base(options)
    {
    }

    public DbSet<OrderEntity> Orders { get; set; } = null!;
    public DbSet<OrderItem> OrderItems { get; set; } = null!;
    
    // Saga state for EF Core repository
    public DbSet<OrderSagaState> OrderSagaStates { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Order configuration
        modelBuilder.Entity<OrderEntity>(entity =>
        {
            entity.ToTable("Orders");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.CustomerId).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.TotalAmount).HasColumnType("decimal(18,2)");
            entity.Property(e => e.OrderDate).IsRequired();

            entity.HasMany(e => e.OrderItems)
                .WithOne()
                .HasForeignKey("OrderId")
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.ToTable("OrderItems");
            entity.HasKey(e => e.Id);
            
            entity.Property(e => e.ProductId).IsRequired();
            entity.Property(e => e.Quantity).IsRequired();
            entity.Property(e => e.UnitPrice).HasColumnType("decimal(18,2)");
        });

        // Saga state configuration for MassTransit
        modelBuilder.Entity<OrderSagaState>(entity =>
        {
            entity.ToTable("OrderSagaStates");
            entity.HasKey(e => e.CorrelationId);
            
            entity.Property(e => e.CurrentState).HasMaxLength(64);
            entity.Property(e => e.ProductsJson).HasColumnType("jsonb");
        });
    }
}
