using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ChatMessageReactionConfiguration : IEntityTypeConfiguration<ChatMessageReaction>
    {
        public void Configure(EntityTypeBuilder<ChatMessageReaction> builder)
        {
            builder.ToTable("ChatMessageReactions");
            builder.HasKey(r => r.ChatMessageReactionId);

            builder.Property(r => r.ReactionType)
                .HasConversion<string>()
                .HasMaxLength(20);

            // One reaction per person per message - re-reacting overwrites, matching
            // PostReaction's precedent.
            builder.HasIndex(r => new { r.ChatMessageId, r.EmployeeId }).IsUnique();

            builder.HasOne<ChatMessage>()
                .WithMany()
                .HasForeignKey(r => r.ChatMessageId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
