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
            builder.ToTable("RfqQuotes");
            builder.HasKey(q => q.Id);

            builder.Property(q => q.UnitPrice).HasPrecision(18, 4);
            builder.Property(q => q.Currency).IsRequired().HasMaxLength(3);
            builder.Property(q => q.Notes).HasMaxLength(2000);
            builder.Property(q => q.PaymentTerms).HasMaxLength(500);
            builder.Property(q => q.DeliveryTerms).HasMaxLength(500);

            builder.HasIndex(q => q.RfqRequestId);
        }
      
    }
 }
