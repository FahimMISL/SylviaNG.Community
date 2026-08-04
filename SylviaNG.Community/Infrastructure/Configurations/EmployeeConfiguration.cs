using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");
            builder.HasKey(e => e.EmployeeId);

            builder.Property(e => e.EmployeeName)
                .HasMaxLength(200);

            builder.Property(e => e.EmployeeCode)
                .HasMaxLength(50);

            builder.Property(e => e.Email)
                .HasMaxLength(200);

            builder.Property(e => e.Phone)
                .HasMaxLength(50);

            builder.Property(e => e.Extension)
                .HasMaxLength(20);

            builder.Property(e => e.PhoneVisibility)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(e => e.EmailVisibility)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(e => e.ExtensionVisibility)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(e => e.Division)
                .HasMaxLength(200);

            builder.Property(e => e.Bio)
                .HasColumnType("text");

            builder.Property(e => e.Skills)
                .HasColumnType("text");

            builder.Property(e => e.Interests)
                .HasColumnType("text");

            builder.Property(e => e.Achievements)
                .HasColumnType("text");

            builder.Property(e => e.CommunityContributions)
                .HasColumnType("text");

            builder.Property(e => e.PhotoUrl)
                .HasMaxLength(500);

            builder.Property(e => e.CoverPhotoUrl)
                .HasMaxLength(500);

            // Indexes
            builder.HasIndex(e => e.EmployeeCode);
            builder.HasIndex(e => e.IsActive);
            builder.HasIndex(e => e.DepartmentId);
            builder.HasIndex(e => e.SiteId);

            // Seed data - dev personas for the Feature 1 mock current-user switcher
            builder.HasData(
                new
                {
                    EmployeeId = 1L,
                    EmployeeName = "Ayesha Rahman",
                    EmployeeCode = "EMP00001",
                    DepartmentId = (long?)1,
                    DesignatioId = (long?)1,
                    SiteId = (long?)1,
                    RFId = (long?)null,
                    Email = "ayesha.rahman@sylviang.example",
                    Phone = "+880-1710-000001",
                    Extension = "1001",
                    PhoneVisibility = ContactVisibilityEnum.Private,
                    EmailVisibility = ContactVisibilityEnum.Private,
                    ExtensionVisibility = ContactVisibilityEnum.Private,
                    GradeId = (long?)null,
                    Division = "Engineering",
                    Bio = "Frontend engineer who enjoys mentoring new hires.",
                    Skills = "Angular, TypeScript, PrimeNG",
                    Interests = "Photography, Chess",
                    Achievements = (string?)null,
                    CommunityContributions = (string?)null,
                    PhotoUrl = (string?)null,
                    CoverPhotoUrl = (string?)null,
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = (long?)null,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    Status = 1
                },
                new
                {
                    EmployeeId = 2L,
                    EmployeeName = "Tanvir Hasan",
                    EmployeeCode = "EMP00002",
                    DepartmentId = (long?)1,
                    DesignatioId = (long?)2,
                    SiteId = (long?)1,
                    RFId = (long?)null,
                    Email = "tanvir.hasan@sylviang.example",
                    Phone = "+880-1710-000002",
                    Extension = "1002",
                    PhoneVisibility = ContactVisibilityEnum.Public,
                    EmailVisibility = ContactVisibilityEnum.Public,
                    ExtensionVisibility = ContactVisibilityEnum.Private,
                    GradeId = (long?)null,
                    Division = "Engineering",
                    Bio = "Engineering team lead focused on delivery quality.",
                    Skills = "Leadership, .NET, SQL",
                    Interests = "Cricket, Reading",
                    Achievements = (string?)null,
                    CommunityContributions = (string?)null,
                    PhotoUrl = (string?)null,
                    CoverPhotoUrl = (string?)null,
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = (long?)null,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    Status = 1
                },
                new
                {
                    EmployeeId = 3L,
                    EmployeeName = "Farhana Akter",
                    EmployeeCode = "EMP00003",
                    DepartmentId = (long?)2,
                    DesignatioId = (long?)3,
                    SiteId = (long?)1,
                    RFId = (long?)null,
                    Email = "farhana.akter@sylviang.example",
                    Phone = "+880-1710-000003",
                    Extension = "1003",
                    PhoneVisibility = ContactVisibilityEnum.Private,
                    EmailVisibility = ContactVisibilityEnum.Public,
                    ExtensionVisibility = ContactVisibilityEnum.Private,
                    GradeId = (long?)null,
                    Division = "People & Culture",
                    Bio = "HR Business Partner supporting the engineering org.",
                    Skills = "Recruiting, Employee Relations",
                    Interests = "Yoga, Travel",
                    Achievements = (string?)null,
                    CommunityContributions = (string?)null,
                    PhotoUrl = (string?)null,
                    CoverPhotoUrl = (string?)null,
                    IsActive = true,
                    TenantId = "default_tenant",
                    Remarks = (string?)null,
                    CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                    CreatedBy = (long?)null,
                    UpdatedAt = (DateTime?)null,
                    UpdatedBy = (long?)null,
                    DeletedAt = (DateTime?)null,
                    DeletedBy = (long?)null,
                    Status = 1
                }
            );
        }
    }
}
