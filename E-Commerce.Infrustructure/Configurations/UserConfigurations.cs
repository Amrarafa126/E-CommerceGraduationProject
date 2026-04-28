using E_Commerce.Data.Entity;
using E_Commerce.Data.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{

    public class ApplicationUserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {  // Rename AspNetUsers → Users (cleaner schema)
            builder.ToTable("Users");

            // ── Domain-only columns ───────────────────────────────────
            builder.Property(u => u.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(u => u.Role)
                .HasConversion<string>()
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(u => u.AvatarUrl)
                .HasMaxLength(1000);

            builder.Property(u => u.RefreshToken)
                .HasMaxLength(500);

            builder.Property(u => u.CreatedAt)
                .IsRequired();

            builder.Property(u => u.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            builder.Property(u => u.IsDeleted)
                .IsRequired()
                .HasDefaultValue(false);

            // ── Relations ─────────────────────────────────────────────

            // Seller → OwnedCompany (1-to-1)
            builder.HasOne(u => u.OwnedCompany)
                .WithOne(c => c.Owner)
                .HasForeignKey<Company>(c => c.OwnerUserId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}

