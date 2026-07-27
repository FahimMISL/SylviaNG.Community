using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ContentReportConfiguration : IEntityTypeConfiguration<ContentReport>
    {
        public void Configure(EntityTypeBuilder<ContentReport> builder)
        {
            builder.ToTable("ContentReports");
            builder.HasKey(r => r.ReportId);

            builder.Property(r => r.Reason)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.PostId);
            builder.HasIndex(r => r.ReportedBy);
            builder.HasIndex(r => r.Status);

            // Reports should survive even if the reported post is later removed, so the
            // moderation trail is preserved - Restrict rather than Cascade.
            builder.HasOne<Post>()
                .WithMany()
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
