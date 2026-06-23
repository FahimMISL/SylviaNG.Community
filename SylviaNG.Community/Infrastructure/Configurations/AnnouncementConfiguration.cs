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
            builder.HasKey(j => j.AnnouncementId);

            builder.Property(j => j.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(j => j.Description)
                .HasColumnType("text");

            builder.Property(j => j.Requirements)
                .HasColumnType("text");

            builder.Property(j => j.NumberOfPositions)
                .HasDefaultValue(1);

            builder.Property(j => j.EmploymentType)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.Status)
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(j => j.MinSalary)
                .HasColumnType("decimal(18,2)");

            builder.Property(j => j.MaxSalary)
                .HasColumnType("decimal(18,2)");

            // Indexes
            builder.HasIndex(j => j.SiteId);
            builder.HasIndex(j => j.Status);
            builder.HasIndex(j => new { j.SiteId, j.Title }).IsUnique();

            // Relationships
            builder.HasMany(j => j.Applications)
                .WithOne(a => a.Announcement)
                .HasForeignKey(a => a.AnnouncementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed data
            builder.HasData(
                new
                {
                    AnnouncementId = 1L,
                    SiteId = 1L,
                    DepartmentId = 1L,
                    DesignationId = 1L,
                    Title = "Senior Software Engineer",
                    Description = "We are looking for a Senior Software Engineer to join our team.",
                    Requirements = "5+ years of experience in .NET, C#, and SQL Server.",
                    NumberOfPositions = 2,
                    EmploymentType = EmploymentTypeEnum.FullTime,
                    Status = JobStatusEnum.Open,
                    MinSalary = 80000m,
                    MaxSalary = 120000m,
                    PostingDate = new DateTime(2025, 1, 1),
                    ClosingDate = new DateTime(2025, 6, 30),
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 1, 1),
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
                    DepartmentId = 2L,
                    DesignationId = 2L,
                    Title = "UI/UX Designer",
                    Description = "Looking for a creative UI/UX Designer.",
                    Requirements = "3+ years of experience in Figma and Adobe XD.",
                    NumberOfPositions = 1,
                    EmploymentType = EmploymentTypeEnum.FullTime,
                    Status = JobStatusEnum.Open,
                    MinSalary = 50000m,
                    MaxSalary = 80000m,
                    PostingDate = new DateTime(2025, 2, 1),
                    ClosingDate = new DateTime(2025, 7, 31),
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 2, 1),
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
