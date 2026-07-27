using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ElectionConfiguration : IEntityTypeConfiguration<Election>
    {
        public void Configure(EntityTypeBuilder<Election> builder)
        {
            builder.ToTable("Elections");
            builder.HasKey(e => e.ElectionId);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(e => e.Description)
                .HasColumnType("text");

            builder.Property(e => e.ElectionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.CandidateType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.AudienceScope)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.Status);
            builder.HasIndex(e => e.StartDate);
            builder.HasIndex(e => e.CreatedBy);
        }
    }
}
