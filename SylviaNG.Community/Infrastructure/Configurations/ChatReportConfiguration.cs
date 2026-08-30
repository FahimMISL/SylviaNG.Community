using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ChatReportConfiguration : IEntityTypeConfiguration<ChatReport>
    {
        public void Configure(EntityTypeBuilder<ChatReport> builder)
        {
            builder.ToTable("ChatReports");
            builder.HasKey(r => r.ChatReportId);

            builder.Property(r => r.Reason)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.ChatConversationId);
            builder.HasIndex(r => r.ReportedByEmployeeId);
            builder.HasIndex(r => r.Status);

            // Reports should survive even if the reported conversation is later removed, so
            // the moderation trail is preserved - Restrict rather than Cascade, matching
            // ContentReportConfiguration's precedent for Posts.
            builder.HasOne<ChatConversation>()
                .WithMany()
                .HasForeignKey(r => r.ChatConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<ChatMessage>()
                .WithMany()
                .HasForeignKey(r => r.ChatMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
