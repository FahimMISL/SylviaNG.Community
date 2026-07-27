using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ConversationConfiguration : IEntityTypeConfiguration<Conversation>
    {
        public void Configure(EntityTypeBuilder<Conversation> builder)
        {
            builder.ToTable("Conversations");
            builder.HasKey(c => c.ConversationId);

            builder.Property(c => c.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(c => c.ListingId);

            builder.HasOne<Listing>()
                .WithMany()
                .HasForeignKey(c => c.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
