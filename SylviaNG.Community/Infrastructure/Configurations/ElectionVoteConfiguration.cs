using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ElectionVoteConfiguration : IEntityTypeConfiguration<ElectionVote>
    {
        public void Configure(EntityTypeBuilder<ElectionVote> builder)
        {
            builder.ToTable("ElectionVotes");
            builder.HasKey(v => v.ElectionVoteId);

            builder.HasIndex(v => v.ElectionId);
            builder.HasIndex(v => v.CandidateId);
            builder.HasIndex(v => v.VoterId);

            builder.HasOne<Election>()
                .WithMany()
                .HasForeignKey(v => v.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Restrict (not Cascade) to avoid a second cascade delete path into this table:
            // Election -> ElectionVote is already Cascade above, and Election -> ElectionCandidate
            // is also Cascade, so ElectionCandidate -> ElectionVote must not also cascade.
            builder.HasOne<ElectionCandidate>()
                .WithMany()
                .HasForeignKey(v => v.CandidateId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
