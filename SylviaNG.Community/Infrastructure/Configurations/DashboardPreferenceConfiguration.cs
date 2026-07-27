using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class DashboardPreferenceConfiguration : IEntityTypeConfiguration<DashboardPreference>
    {
        public void Configure(EntityTypeBuilder<DashboardPreference> builder)
        {
            builder.ToTable("DashboardPreferences");
            builder.HasKey(d => d.PreferenceId);

            builder.Property(d => d.WidgetName)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(d => new { d.EmployeeId, d.WidgetName }).IsUnique();
        }
    }
}
