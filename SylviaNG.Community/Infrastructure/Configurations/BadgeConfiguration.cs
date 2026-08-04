using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class BadgeConfiguration : IEntityTypeConfiguration<Badge>
    {
        public void Configure(EntityTypeBuilder<Badge> builder)
        {
            builder.ToTable("Badges");
            builder.HasKey(b => b.BadgeId);

            builder.Property(b => b.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(b => b.Icon)
                .HasMaxLength(300);

            builder.Property(b => b.Description)
                .HasColumnType("text");

            builder.Property(b => b.Color)
                .HasMaxLength(20);

            builder.Property(b => b.IsActive)
                .HasDefaultValue(true);

            builder.HasIndex(b => b.Name).IsUnique();
        }
    }
}
