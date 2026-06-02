using E_Commerce.Data.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace E_Commerce.Infrustructure.Configurations
{
    public class ReviewHelpfulVoteConfiguration : IEntityTypeConfiguration<ReviewHelpfulVote>
    {
        public void Configure(EntityTypeBuilder<ReviewHelpfulVote> builder)
        {
            builder.ToTable("ReviewHelpfulVotes");
            builder.HasKey(hv => hv.Id);

            builder.HasOne(hv => hv.Review)
                .WithMany(r => r.HelpfulVotes)
                .HasForeignKey(hv => hv.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(hv => hv.Reviewer)
                .WithMany()
                .HasForeignKey(hv => hv.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(hv => new { hv.ReviewId, hv.ReviewerId }).IsUnique();
        }
    }
}
