using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class RecognitionBadgeConfiguration : IEntityTypeConfiguration<RecognitionBadge>
    {
        public void Configure(EntityTypeBuilder<RecognitionBadge> builder)
        {
            builder.ToTable("RecognitionBadges");
            builder.HasKey(x => x.RecognitionBadgeId);

            builder.HasIndex(x => new { x.RecognitionId, x.BadgeId }).IsUnique();
            builder.HasIndex(x => x.BadgeId);

            builder.HasOne<Recognition>()
                .WithMany()
                .HasForeignKey(x => x.RecognitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Badge>()
                .WithMany()
                .HasForeignKey(x => x.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
