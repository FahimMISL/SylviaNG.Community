using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ConversationParticipantConfiguration : IEntityTypeConfiguration<ConversationParticipant>
    {
        public void Configure(EntityTypeBuilder<ConversationParticipant> builder)
        {
            builder.ToTable("ConversationParticipants");
            builder.HasKey(cp => cp.ParticipantId);

            builder.HasIndex(cp => new { cp.ConversationId, cp.EmployeeId }).IsUnique();
            builder.HasIndex(cp => cp.EmployeeId);

            builder.HasOne<Conversation>()
                .WithMany()
                .HasForeignKey(cp => cp.ConversationId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
