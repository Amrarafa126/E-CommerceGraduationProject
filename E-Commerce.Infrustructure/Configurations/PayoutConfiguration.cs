using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrustructure.Configurations
{
    public class PayoutConfiguration : IEntityTypeConfiguration<Payout>
    {
        public void Configure(EntityTypeBuilder<Payout> builder)
        {
            builder.ToTable("Payouts");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Amount).HasPrecision(18, 4);
            builder.Property(p => p.Currency).IsRequired().HasMaxLength(3);
            builder.Property(p => p.Status).HasConversion<string>().HasMaxLength(20);
            builder.Property(p => p.ExternalReference).HasMaxLength(200);
            builder.Property(p => p.BankAccountLast4).HasMaxLength(10);
            builder.Property(p => p.FailureReason).HasMaxLength(500);
        }
    }
}
