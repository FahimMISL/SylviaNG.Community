using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.ToTable("Notifications");
            builder.HasKey(n => n.NotificationId);

            builder.Property(n => n.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(n => n.Message)
                .HasColumnType("text");

            builder.Property(n => n.Category)
                .HasMaxLength(100);

            builder.Property(n => n.RelatedEntityType)
                .HasMaxLength(100);

            builder.HasIndex(n => n.EmployeeId);
            builder.HasIndex(n => n.IsRead);
            builder.HasIndex(n => n.Category);
        }
    }
}
