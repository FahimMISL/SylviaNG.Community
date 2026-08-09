using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class EmployeeContactLinkConfiguration : IEntityTypeConfiguration<EmployeeContactLink>
    {
        public void Configure(EntityTypeBuilder<EmployeeContactLink> builder)
        {
            builder.ToTable("EmployeeContactLinks");
            builder.HasKey(e => e.EmployeeContactLinkId);

            builder.Property(e => e.Platform)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Url)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Visibility)
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(e => e.EmployeeId);

            builder.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
