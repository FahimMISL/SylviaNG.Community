using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class EmployeeKeycloakAccountConfiguration : IEntityTypeConfiguration<EmployeeKeycloakAccount>
    {
        public void Configure(EntityTypeBuilder<EmployeeKeycloakAccount> builder)
        {
            builder.ToTable("EmployeeKeycloakAccounts");
            builder.HasKey(e => e.EmployeeKeycloakAccountId);

            builder.Property(e => e.KeycloakUserId)
                .IsRequired()
                .HasMaxLength(64);

            builder.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.AssignedRole)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.EmployeeId).IsUnique();
            builder.HasIndex(e => e.Username).IsUnique();
        }
    }
}
