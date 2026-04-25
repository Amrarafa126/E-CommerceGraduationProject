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
  
    public class QuotationConfigurations : IEntityTypeConfiguration<RfqQuotation>
        {
         public void Configure(EntityTypeBuilder<RfqQuotation> builder)
         {
            //builder.ToTable("RfqQuotations");
            //    builder.HasKey(q => q.Id);
            //    builder.Property(q => q.QuotationNumber).IsRequired().HasMaxLength(50);
            //    builder.Property(q => q.TotalAmount).HasPrecision(18, 4);
            //    builder.Property(q => q.Currency).IsRequired().HasMaxLength(3);
            //    builder.Property(q => q.Notes).HasMaxLength(2000);
            //    builder.Property(q => q.Status).HasConversion<string>().HasMaxLength(20);
    
            //    builder.HasIndex(q => q.QuotationNumber).IsUnique();
    
            //    builder.HasOne(q => q.Rfq)
            //        .WithMany(r => r.Quotations)
            //        .HasForeignKey(q => q.RfqId)
            //        .OnDelete(DeleteBehavior.Cascade);


        }
      
    }
 }
