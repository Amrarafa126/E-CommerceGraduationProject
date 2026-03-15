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
  
    public class QuotationConfigurations : IEntityTypeConfiguration<Quotation>
        {
         public void Configure(EntityTypeBuilder<Quotation> builder)
         {
              
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Price)
                       .HasColumnType("decimal(18,2)")
                       .IsRequired();

            builder.Property(x => x.Message)
                       .HasMaxLength(1000);

            builder.Property(x => x.CreatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .ValueGeneratedOnAdd();

            builder.HasOne(x => x.RFQ)
                   .WithMany(x => x.Quotations)
                   .HasForeignKey(x => x.RFQId);
            // .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Supplier)
                   .WithMany()
                   .HasForeignKey(x => x.SupplierId);
                      // .OnDelete(DeleteBehavior.Restrict); 

                builder.HasIndex(x => x.RFQId);
                builder.HasIndex(x => x.SupplierId);
          }
      
    }
 }
