
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
  
        public class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
        {
            public void Configure(EntityTypeBuilder<OrderItem> builder)
            {
                builder.HasKey(x => x.OrderItemId);

                builder.Property(x => x.Quantity)
                       .IsRequired();

                builder.Property(x => x.Price)
                       .HasColumnType("decimal(18,1)")
                       .IsRequired();

            builder.HasOne(x => x.Order)
                   .WithMany(x => x.orderItems)
                   .HasForeignKey(x => x.OrderId);
            // .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.ProductVariant)
               .WithMany(x => x.OrderItems)
               .HasForeignKey(x => x.ProductVariantId);
                  // .OnDelete(DeleteBehavior.Restrict);

      
                builder.HasIndex(x => x.OrderId);
                builder.HasIndex(x => x.ProductVariantId);
            }

      
        }
}

