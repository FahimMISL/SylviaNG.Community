using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PostAttachmentConfiguration : IEntityTypeConfiguration<PostAttachment>
    {
        public void Configure(EntityTypeBuilder<PostAttachment> builder)
        {
            builder.ToTable("PostAttachments");
            builder.HasKey(a => a.AttachmentId);

            builder.Property(a => a.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(a => a.FileType)
                .HasMaxLength(100);

            builder.Property(a => a.FilePath)
                .IsRequired()
                .HasMaxLength(1000);

            builder.HasIndex(a => a.PostId);

            builder.HasOne<Post>()
                .WithMany()
                .HasForeignKey(a => a.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Never let an attachment silently orphan if the underlying file record is
            // removed - same reasoning as ChatMessageAttachment's restrict-on-delete.
            builder.HasOne<FileStorage>()
                .WithMany()
                .HasForeignKey(a => a.FileStorageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
