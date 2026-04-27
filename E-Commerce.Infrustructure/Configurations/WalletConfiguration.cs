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
    public class WalletConfiguration : IEntityTypeConfiguration<Wallet>
    {
        public void Configure(EntityTypeBuilder<Wallet> builder)
        {
            builder.ToTable("Wallets");
            builder.HasKey(w => w.Id);
            builder.Property(w => w.PendingBalance).HasPrecision(18, 4);
            builder.Property(w => w.AvailableBalance).HasPrecision(18, 4);
            builder.Property(w => w.TotalEarned).HasPrecision(18, 4);
            builder.Property(w => w.Currency).IsRequired().HasMaxLength(3);
            builder.HasIndex(w => w.CompanyId).IsUnique();
        }
    }
}
