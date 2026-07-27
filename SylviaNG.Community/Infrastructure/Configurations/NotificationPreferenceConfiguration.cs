using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
    {
        public void Configure(EntityTypeBuilder<NotificationPreference> builder)
        {
            builder.ToTable("NotificationPreferences");
            builder.HasKey(p => p.PreferenceId);

            builder.Property(p => p.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => new { p.EmployeeId, p.Category }).IsUnique();
        }
    }
}
