using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class OrderConfigurations : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
           
            builder.ToTable("Orders");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TotalPrice)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            builder.Property(x => x.Status)
                   .HasMaxLength(50)
                   .IsRequired();

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(x => x.Buyer)
                   .WithMany(x => x.Orders)
                   .HasForeignKey(x => x.BuyerId);

            builder.HasMany(x => x.orderItems)
                   .WithOne(x => x.Order)
                   .HasForeignKey(x => x.OrderId);

            builder.HasIndex(x => x.BuyerId);
        }
    }
}

