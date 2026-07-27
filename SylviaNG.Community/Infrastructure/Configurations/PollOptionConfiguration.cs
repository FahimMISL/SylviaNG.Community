using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PollOptionConfiguration : IEntityTypeConfiguration<PollOption>
    {
        public void Configure(EntityTypeBuilder<PollOption> builder)
        {
            builder.ToTable("PollOptions");
            builder.HasKey(o => o.PollOptionId);

            builder.Property(o => o.OptionText)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(o => o.PollId);

            builder.HasOne<Poll>()
                .WithMany()
                .HasForeignKey(o => o.PollId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
