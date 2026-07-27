using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class RecognitionReactionConfiguration : IEntityTypeConfiguration<RecognitionReaction>
    {
        public void Configure(EntityTypeBuilder<RecognitionReaction> builder)
        {
            builder.ToTable("RecognitionReactions");
            builder.HasKey(rr => rr.ReactionId);

            builder.Property(rr => rr.ReactionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(rr => new { rr.RecognitionId, rr.EmployeeId }).IsUnique();

            builder.HasOne<Recognition>()
                .WithMany()
                .HasForeignKey(rr => rr.RecognitionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
