using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class MentionConfiguration : IEntityTypeConfiguration<Mention>
    {
        public void Configure(EntityTypeBuilder<Mention> builder)
        {
            builder.ToTable("Mentions");
            builder.HasKey(m => m.MentionId);

            builder.Property(m => m.EntityType)
                .IsRequired()
                .HasMaxLength(50);

            // EntityId is a generic polymorphic pointer (Post or PostComment depending on
            // EntityType) - intentionally no FK constraint.
            builder.HasIndex(m => m.MentionedEmployeeId);
            builder.HasIndex(m => m.MentionedBy);
            builder.HasIndex(m => new { m.EntityType, m.EntityId });
        }
    }
}
