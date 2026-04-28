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
       public class RfqConfigurations : IEntityTypeConfiguration<RfqRequest>
        {
            public void Configure(EntityTypeBuilder<RfqRequest> builder)
            {

            builder.ToTable("RfqRequests");
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Title).IsRequired().HasMaxLength(300);
            builder.Property(r => r.Description).IsRequired().HasMaxLength(3000);
            builder.Property(r => r.Currency).IsRequired().HasMaxLength(3);
            builder.Property(r => r.TargetPrice).HasMaxLength(100);
            builder.Property(r => r.ShippingCountry).HasMaxLength(100);
            builder.Property(r => r.Status).HasConversion<string>().HasMaxLength(20);

            builder.HasOne(r => r.Buyer)
                .WithMany()
                .HasForeignKey(r => r.BuyerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.SellerCompany)
                .WithMany()
                .HasForeignKey(r => r.SellerCompanyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey(r => r.ProductId)
                .OnDelete(DeleteBehavior.SetNull)
                .IsRequired(false);

            builder.HasMany(r => r.Quotes)
                .WithOne(q => q.RfqRequest)
                .HasForeignKey(q => q.RfqRequestId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(r => r.BuyerId);
            builder.HasIndex(r => r.SellerCompanyId);
            builder.HasIndex(r => r.Status);
        }
    }
 }
