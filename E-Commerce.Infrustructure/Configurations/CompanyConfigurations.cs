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
    public class CompanyConfigurations : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.CompanyId);

            builder.Property(x => x.CompanyName)
                   .HasMaxLength(200)
                   .IsRequired();

            builder.Property(x => x.Email)
                   .HasMaxLength(200);

            builder.Property(x => x.Phone)
                   .HasMaxLength(20);

            builder.Property(x => x.Address)
                   .HasMaxLength(500);

            builder.Property(x => x.Description)
                   .HasMaxLength(1000);

            builder.Property(x => x.VerificationStatus)
                   .HasDefaultValue(false);


            builder.HasMany(x => x.Users)
                   .WithOne(x => x.Company)
                   .HasForeignKey(x => x.CompanyId);

            builder.HasMany(x => x.Products)
                   .WithOne(x => x.Company)
                   .HasForeignKey(x => x.CompanyId);

            builder.HasIndex(x => x.Email)
                   .IsUnique(false);

        }
    }
}
