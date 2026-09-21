using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ChatConversationConfiguration : IEntityTypeConfiguration<ChatConversation>
    {
        public void Configure(EntityTypeBuilder<ChatConversation> builder)
        {
            builder.ToTable("ChatConversations");
            builder.HasKey(c => c.ChatConversationId);

            builder.Property(c => c.Type)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(c => c.Title)
                .HasMaxLength(200);

            builder.Property(c => c.LastMessagePreview)
                .HasMaxLength(200);

            builder.HasIndex(c => c.CreatedByEmployeeId);
            builder.HasIndex(c => c.LastMessageAt);

            // Never let a group avatar silently orphan if the underlying file record is
            // removed - same reasoning as ChatMessageAttachment's restrict-on-delete. This
            // was previously a "soft" reference validated only at the service layer
            // (ChatConversationService) with no DB-level constraint.
            builder.HasOne<FileStorage>()
                .WithMany()
                .HasForeignKey(c => c.GroupAvatarFileId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
