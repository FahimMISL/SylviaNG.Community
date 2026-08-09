using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.ToTable("Branches");
            builder.HasKey(b => b.BranchId);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Address)
                .HasColumnType("text");

            builder.Property(b => b.City)
                .HasMaxLength(100);

            builder.Property(b => b.Country)
                .HasMaxLength(100);

            builder.HasIndex(b => b.IsActive);
        }
    }
}
