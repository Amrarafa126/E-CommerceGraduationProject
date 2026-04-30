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
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.ToTable("Companies");
            builder.HasKey(c => c.Id);

            builder.Property(c => c.CompanyName).IsRequired().HasMaxLength(200);
            builder.Property(c => c.Description).IsRequired().HasMaxLength(2000);
            builder.Property(c => c.LogoUrl).HasMaxLength(1000);
            builder.Property(c => c.Status)
                .HasConversion<string>().HasMaxLength(20);

            builder.OwnsOne(c => c.Address, a =>
            {
                a.Property(x => x.Street).HasColumnName("Street").IsRequired().HasMaxLength(300);
                a.Property(x => x.City).HasColumnName("City").IsRequired().HasMaxLength(100);
                a.Property(x => x.State).HasColumnName("State").HasMaxLength(100);
                a.Property(x => x.Country).HasColumnName("Country").IsRequired().HasMaxLength(100);
                a.Property(x => x.PostalCode).HasColumnName("PostalCode").HasMaxLength(20);
            });

            builder.OwnsOne(c => c.ContactInfo, ci =>
            {
                ci.Property(x => x.Email).HasColumnName("ContactEmail").IsRequired().HasMaxLength(256);
                ci.Property(x => x.Phone).HasColumnName("ContactPhone").IsRequired().HasMaxLength(20);
            });

            builder.HasMany(c => c.Products)
                .WithOne(p => p.Company)
                .HasForeignKey(p => p.CompanyId)
                .OnDelete(DeleteBehavior.Cascade);//يحذف الكل تلقائيًا
        }
    }
}
