using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class InterestConfiguration : IEntityTypeConfiguration<Interest>
    {
        public void Configure(EntityTypeBuilder<Interest> builder)
        {
            builder.ToTable("Interests");
            builder.HasKey(i => i.InterestId);

            builder.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(200);

            builder.HasIndex(i => i.Name).IsUnique();
        }
    }
}
