using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ChatMessageAttachmentConfiguration : IEntityTypeConfiguration<ChatMessageAttachment>
    {
        public void Configure(EntityTypeBuilder<ChatMessageAttachment> builder)
        {
            builder.ToTable("ChatMessageAttachments");
            builder.HasKey(a => a.ChatMessageAttachmentId);

            builder.Property(a => a.AttachmentType)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(a => a.ChatMessageId);

            builder.HasOne<ChatMessage>()
                .WithMany()
                .HasForeignKey(a => a.ChatMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Never let an attachment silently orphan if the underlying file record is
            // removed - same reasoning as ContentReport's restrict-on-delete.
            builder.HasOne<FileStorage>()
                .WithMany()
                .HasForeignKey(a => a.FileStorageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
