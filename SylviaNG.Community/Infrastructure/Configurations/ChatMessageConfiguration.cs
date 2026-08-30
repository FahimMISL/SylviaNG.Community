using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ChatMessageConfiguration : IEntityTypeConfiguration<ChatMessage>
    {
        public void Configure(EntityTypeBuilder<ChatMessage> builder)
        {
            builder.ToTable("ChatMessages");
            builder.HasKey(m => m.ChatMessageId);

            builder.Property(m => m.Body)
                .HasColumnType("text");

            builder.Property(m => m.MessageType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(m => m.SharedContentType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(m => new { m.ChatConversationId, m.SentAt });
            builder.HasIndex(m => m.SenderEmployeeId);

            // Messages survive their conversation being referenced elsewhere but belong
            // strictly to one conversation - restrict rather than cascade, consistent with
            // ChatParticipant, so a conversation can't be deleted out from under its history.
            builder.HasOne<ChatConversation>()
                .WithMany()
                .HasForeignKey(m => m.ChatConversationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Self-referencing reply pointer. Restrict (not Cascade/SetNull) is safe here since
            // messages are never hard-deleted - "removed" is the IsDeleted flag above, which
            // keeps the row (and this FK) intact.
            builder.HasOne<ChatMessage>()
                .WithMany()
                .HasForeignKey(m => m.ReplyToMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
