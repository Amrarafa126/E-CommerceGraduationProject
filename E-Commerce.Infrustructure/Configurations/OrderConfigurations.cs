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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);
            builder.Property(o => o.OrderNumber).IsRequired().HasMaxLength(50);
            builder.Property(o => o.TotalAmount).HasPrecision(18, 4);
            builder.Property(o => o.Currency).IsRequired().HasMaxLength(3);
            builder.Property(o => o.Notes).HasMaxLength(2000);
            builder.Property(o => o.ShippingAddress).HasMaxLength(500);
            builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(o => o.PaymentStatus).HasConversion<string>().HasMaxLength(20);

            builder.HasIndex(o => o.OrderNumber).IsUnique();

            builder.HasOne(o => o.Buyer)
                .WithMany(u => u.Orders)
                .HasForeignKey(o => o.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(o => o.Supplier)
                .WithMany()
                .HasForeignKey(o => o.SupplierId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(o => o.Items)
                .WithOne(i => i.Order)
                .HasForeignKey(i => i.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
            // Payment: optional 1-to-1
            //builder.HasOne(o => o.Payment)
            //    .WithOne(p => p.Order).HasForeignKey<Payment>(p => p.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .IsRequired(false);

            // Shipment: optional 1-to-1
            //builder.HasOne(o => o.Shipment)
            //    .WithOne(s => s.Order).HasForeignKey<Shipment>(s => s.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade)
            //    .IsRequired(false);

            //builder.HasMany(o => o.Items)
            //    .WithOne(i => i.Order).HasForeignKey(i => i.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //builder.HasMany(o => o.StatusHistory)
            //    .WithOne(h => h.Order).HasForeignKey(h => h.OrderId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

