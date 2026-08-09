using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class EmployeeBadgeConfiguration : IEntityTypeConfiguration<EmployeeBadge>
    {
        public void Configure(EntityTypeBuilder<EmployeeBadge> builder)
        {
            builder.ToTable("EmployeeBadges");
            builder.HasKey(eb => eb.EmployeeBadgeId);

            builder.HasIndex(eb => new { eb.EmployeeId, eb.BadgeId });
            builder.HasIndex(eb => eb.EmployeeId);

            builder.HasOne<Badge>()
                .WithMany()
                .HasForeignKey(eb => eb.BadgeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
