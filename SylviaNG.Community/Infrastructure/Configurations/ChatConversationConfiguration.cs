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
        }
    }
}
