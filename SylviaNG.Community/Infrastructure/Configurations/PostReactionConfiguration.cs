using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PostReactionConfiguration : IEntityTypeConfiguration<PostReaction>
    {
        public void Configure(EntityTypeBuilder<PostReaction> builder)
        {
            builder.ToTable("PostReactions");
            builder.HasKey(r => r.ReactionId);

            builder.Property(r => r.ReactionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => new { r.PostId, r.EmployeeId }).IsUnique();
            builder.HasIndex(r => r.EmployeeId);

            builder.HasOne<Post>()
                .WithMany()
                .HasForeignKey(r => r.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
