using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(d => d.DepartmentId);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Code)
                .HasMaxLength(50);

            builder.Property(d => d.Description)
                .HasColumnType("text");

            builder.HasIndex(d => d.IsActive);

            // Matches StubCoreGrpcClient's DepartmentNames array 1:1 by id, so switching the
            // stub to resolve names from this table (instead of a generated placeholder) doesn't
            // change any already-displayed employee's Department name.
            var seedNames = new[]
            {
                "Engineering", "Human Resources", "Sales & Marketing", "Finance",
                "Operations", "Customer Support", "Legal", "IT Infrastructure"
            };

            builder.HasData(seedNames.Select((name, index) => new
            {
                DepartmentId = (long)(index + 1),
                Name = name,
                Code = (string?)null,
                Description = (string?)null,
                IsActive = true,
                TenantId = "default_tenant",
                Remarks = (string?)null,
                CreatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = (long?)null,
                UpdatedAt = (DateTime?)null,
                UpdatedBy = (long?)null,
                DeletedAt = (DateTime?)null,
                DeletedBy = (long?)null,
                Status = 0
            }));
        }
    }
}
