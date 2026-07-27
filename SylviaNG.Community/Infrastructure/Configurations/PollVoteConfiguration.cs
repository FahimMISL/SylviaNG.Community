using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PollVoteConfiguration : IEntityTypeConfiguration<PollVote>
    {
        public void Configure(EntityTypeBuilder<PollVote> builder)
        {
            builder.ToTable("PollVotes");
            builder.HasKey(v => v.VoteId);

            builder.HasIndex(v => v.PollOptionId);
            builder.HasIndex(v => v.EmployeeId);

            builder.HasOne<PollOption>()
                .WithMany()
                .HasForeignKey(v => v.PollOptionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
