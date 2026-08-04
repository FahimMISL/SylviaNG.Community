using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class RecognitionConfiguration : IEntityTypeConfiguration<Recognition>
    {
        public void Configure(EntityTypeBuilder<Recognition> builder)
        {
            builder.ToTable("Recognitions");
            builder.HasKey(r => r.RecognitionId);

            builder.Property(r => r.RecognitionType)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(r => r.CoreValue)
                .HasMaxLength(100);

            builder.Property(r => r.AwardTitle)
                .HasMaxLength(200);

            builder.Property(r => r.Message)
                .HasColumnType("text");

            builder.HasIndex(r => r.SenderId);
            builder.HasIndex(r => r.RecipientId);
            builder.HasIndex(r => r.BadgeId);

            builder.HasOne<Badge>()
                .WithMany()
                .HasForeignKey(r => r.BadgeId)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
