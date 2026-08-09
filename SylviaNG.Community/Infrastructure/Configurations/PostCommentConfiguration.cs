using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PostCommentConfiguration : IEntityTypeConfiguration<PostComment>
    {
        public void Configure(EntityTypeBuilder<PostComment> builder)
        {
            builder.ToTable("PostComments");
            builder.HasKey(c => c.CommentId);

            builder.Property(c => c.Content)
                .IsRequired()
                .HasColumnType("text");

            builder.HasIndex(c => c.PostId);
            builder.HasIndex(c => c.EmployeeId);
            builder.HasIndex(c => c.ParentCommentId);

            builder.HasOne<Post>()
                .WithMany()
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Self-referencing FK for threaded replies. Restrict (not Cascade) to avoid
            // multi-path cascade-delete cycles on providers that disallow them (SQL Server).
            builder.HasOne<PostComment>()
                .WithMany()
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
