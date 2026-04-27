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
  
 
        public class RfqConfigurations : IEntityTypeConfiguration<RfqRequest>
        {
            public void Configure(EntityTypeBuilder<RfqRequest> builder)
            {
               
                builder.HasKey(x => x.Id);
         
                builder.Property(x => x.Title)
                       .HasMaxLength(200)
                       .IsRequired();

                builder.Property(x => x.Description)
                       .HasMaxLength(2000);

                builder.Property(x => x.CreatedAt)
                       .HasDefaultValueSql("GETDATE()")
                       .ValueGeneratedOnAdd();

            //builder.HasMany(x => x.rf)
            //       .WithOne(x => x.RFQ)
            //       .HasForeignKey(x => x.RFQId);
                     //  .OnDelete(DeleteBehavior.Restrict);

                // Indexes
                builder.HasIndex(x => x.BuyerId);
            }

   
    }
 }
