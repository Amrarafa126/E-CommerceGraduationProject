using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{

    public class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.FirstName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.LastName).IsRequired().HasMaxLength(100);
            builder.Property(u => u.PhoneNumber).HasMaxLength(20);
            builder.Property(u => u.role).HasConversion<string>().HasMaxLength(20);
            builder.Property(u => u.RefreshToken).HasMaxLength(500);

            builder.HasIndex(u => u.Email).IsUnique();

            //// Owned company (1-to-1, Seller → Company)
            //builder.HasOne(u => u.OwnedCompany)
            //    .WithOne(c => c.Owner)
            //    .HasForeignKey<Company>(c => c.OwnerUserId)
            //    .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

