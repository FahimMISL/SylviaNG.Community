using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class DesignationConfiguration : IEntityTypeConfiguration<Designation>
    {
        public void Configure(EntityTypeBuilder<Designation> builder)
        {
            builder.ToTable("Designations");
            builder.HasKey(d => d.DesignationId);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(d => d.Grade)
                .HasMaxLength(50);

            builder.Property(d => d.Description)
                .HasColumnType("text");

            builder.HasIndex(d => d.IsActive);

            // Matches StubCoreGrpcClient's DesignationNames array 1:1 by id, so switching the
            // stub to resolve names from this table (instead of a generated placeholder) doesn't
            // change any already-displayed employee's Designation name.
            var seedNames = new[]
            {
                "Software Engineer", "Senior Software Engineer", "HR Manager", "Sales Executive",
                "Financial Analyst", "Operations Lead", "Support Specialist", "Legal Counsel",
                "System Administrator", "Team Lead"
            };

            builder.HasData(seedNames.Select((name, index) => new
            {
                DesignationId = (long)(index + 1),
                Name = name,
                Grade = (string?)null,
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
