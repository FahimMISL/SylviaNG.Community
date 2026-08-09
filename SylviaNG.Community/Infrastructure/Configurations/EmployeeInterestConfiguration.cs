using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class EmployeeInterestConfiguration : IEntityTypeConfiguration<EmployeeInterest>
    {
        public void Configure(EntityTypeBuilder<EmployeeInterest> builder)
        {
            builder.ToTable("EmployeeInterests");
            builder.HasKey(ei => ei.EmployeeInterestId);

            builder.HasIndex(ei => new { ei.EmployeeId, ei.InterestId }).IsUnique();
            builder.HasIndex(ei => ei.EmployeeId);

            builder.HasOne<Interest>()
                .WithMany()
                .HasForeignKey(ei => ei.InterestId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
