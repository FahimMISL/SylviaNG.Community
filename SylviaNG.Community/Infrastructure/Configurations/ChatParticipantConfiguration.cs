using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ChatParticipantConfiguration : IEntityTypeConfiguration<ChatParticipant>
    {
        public void Configure(EntityTypeBuilder<ChatParticipant> builder)
        {
            builder.ToTable("ChatParticipants");
            builder.HasKey(p => p.ChatParticipantId);

            builder.HasIndex(p => new { p.ChatConversationId, p.EmployeeId });
            builder.HasIndex(p => p.EmployeeId);

            // Conversations survive a participant leaving/being removed - the row is kept
            // (LeftAt set) to preserve message attribution, so restrict rather than cascade.
            builder.HasOne<ChatConversation>()
                .WithMany()
                .HasForeignKey(p => p.ChatConversationId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
