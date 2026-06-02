using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class MessageConfiguration : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(m => m.Id);
            builder.Property(m => m.Content).IsRequired().HasMaxLength(2000);
            builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);
            builder.Property(m => m.CardDataJson).HasMaxLength(4000);
            builder.Property(m => m.IdempotencyKey).HasMaxLength(64);
            builder.HasIndex(m => m.IdempotencyKey).IsUnique();
            builder.HasIndex(m => new { m.ConversationId, m.SenderId, m.CreatedAt });

            builder.HasOne(m => m.Sender).WithMany()
                .HasForeignKey(m => m.SenderId).OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(m => m.ReplyToMessage).WithMany()
                .HasForeignKey(m => m.ReplyToMessageId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(m => m.Attachments)
                .WithOne(a => a.Message)
                .HasForeignKey(a => a.MessageId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.ReadReceipts)
                .WithOne(r => r.Message)
                .HasForeignKey(r => r.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
