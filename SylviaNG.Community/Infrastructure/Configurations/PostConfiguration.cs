using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PostConfiguration : IEntityTypeConfiguration<Post>
    {
        public void Configure(EntityTypeBuilder<Post> builder)
        {
            builder.ToTable("Posts");
            builder.HasKey(p => p.PostId);

            builder.Property(p => p.Type)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.Visibility)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.Property(p => p.Content)
                .HasColumnType("text");

            builder.HasIndex(p => p.EmployeeId);
            builder.HasIndex(p => p.Type);
            builder.HasIndex(p => p.Visibility);
            builder.HasIndex(p => p.IsHidden);
        }
    }
}
