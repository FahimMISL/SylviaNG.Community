using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
    {
        public void Configure(EntityTypeBuilder<AuditLog> builder)
        {
            builder.ToTable("AuditLogs");
            builder.HasKey(a => a.AuditId);

            builder.Property(a => a.TableName)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Action)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(a => a.OldValue)
                .HasColumnType("text");

            builder.Property(a => a.NewValue)
                .HasColumnType("text");

            builder.HasIndex(a => new { a.TableName, a.RecordId });
            builder.HasIndex(a => a.PerformedBy);
        }
    }
}
