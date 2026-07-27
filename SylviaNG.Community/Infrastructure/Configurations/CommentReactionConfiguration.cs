using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class CommentReactionConfiguration : IEntityTypeConfiguration<CommentReaction>
    {
        public void Configure(EntityTypeBuilder<CommentReaction> builder)
        {
            builder.ToTable("CommentReactions");
            builder.HasKey(r => r.ReactionId);

            builder.Property(r => r.ReactionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => new { r.CommentId, r.EmployeeId }).IsUnique();
            builder.HasIndex(r => r.EmployeeId);

            builder.HasOne<PostComment>()
                .WithMany()
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
