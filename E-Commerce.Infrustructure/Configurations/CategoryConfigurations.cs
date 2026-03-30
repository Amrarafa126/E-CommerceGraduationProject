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
    public class CategoryConfigurations : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(c => c.Id);
            builder.Property(n => n.NameCategory).HasMaxLength(100).IsRequired();



            builder.HasOne(c => c.CategoryParent)
                .WithMany(c => c.CategoryChildren)
                .HasForeignKey(c => c.ParentId)
                .IsRequired(false);
               // .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.ParentId);
        }
    }
}
