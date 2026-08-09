using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ElectionAudienceTargetConfiguration : IEntityTypeConfiguration<ElectionAudienceTarget>
    {
        public void Configure(EntityTypeBuilder<ElectionAudienceTarget> builder)
        {
            builder.ToTable("ElectionAudienceTargets");
            builder.HasKey(t => t.ElectionAudienceTargetId);

            builder.Property(t => t.TargetId)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(t => t.ElectionId);

            builder.HasOne<Election>()
                .WithMany()
                .HasForeignKey(t => t.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
