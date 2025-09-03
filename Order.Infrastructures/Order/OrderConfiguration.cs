using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Order.Domain.Order;

namespace Order.Infrastructures.Order
{
    public class OrderConfiguration : IEntityTypeConfiguration<OrderDb>
    {
        public void Configure(EntityTypeBuilder<OrderDb> builder)
        {
            builder.ToTable("Orders");
            
            builder.HasKey(o => o.Id);
            
            builder.Property(o => o.CustomerName)
                .IsRequired()
                .HasMaxLength(100);
                
            builder.Property(o => o.OrderDate)
                .IsRequired();
                
            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(18,2)");
                
            builder.Property(o => o.Status)
                .IsRequired();
                
            builder.Property(o => o.CreatedAt)
                .IsRequired();
                
            builder.Property(o => o.UpdatedAt)
                .IsRequired();
                
            builder.Property(o => o.IsDeleted)
                .IsRequired();
        }
    }
}
