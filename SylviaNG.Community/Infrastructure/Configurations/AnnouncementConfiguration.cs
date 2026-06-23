using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class AnnouncementConfiguration : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.ToTable("Announcements");
            builder.HasKey(a => a.AnnouncementId);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Body)
                .HasColumnType("text");

            builder.Property(a => a.AnnouncementType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(a => a.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(a => a.SiteId);
            builder.HasIndex(a => a.Status);
            builder.HasIndex(a => new { a.SiteId, a.Title }).IsUnique();

            // Seed data
            builder.HasData(
                new
                {
                    AnnouncementId = 1L,
                    SiteId = 1L,
                    DepartmentId = (long?)null,
                    Title = "Welcome to SylviaNG Community",
                    Body = "Stay connected with company news, events, and announcements.",
                    AnnouncementType = AnnouncementTypeEnum.General,
                    Status = AnnouncementStatusEnum.Published,
                    PublishDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    ExpiryDate = (DateTime?)null,
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = 1L,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    AuditStatus = 1
                },
                new
                {
                    AnnouncementId = 2L,
                    SiteId = 1L,
                    DepartmentId = (long?)null,
                    Title = "Annual Company Picnic",
                    Body = "Join us for our annual company picnic this weekend at the lakeside.",
                    AnnouncementType = AnnouncementTypeEnum.Event,
                    Status = AnnouncementStatusEnum.Scheduled,
                    PublishDate = new DateTime(2025, 6, 15, 0, 0, 0, DateTimeKind.Utc),
                    ExpiryDate = new DateTime(2025, 7, 1, 0, 0, 0, DateTimeKind.Utc),
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 5, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = 1L,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    AuditStatus = 1
                }
            );
        }
    }
}