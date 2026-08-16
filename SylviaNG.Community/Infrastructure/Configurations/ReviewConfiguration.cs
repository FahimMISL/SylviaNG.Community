using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            builder.ToTable("Reviews");
            builder.HasKey(r => r.ReviewId);

            builder.Property(r => r.Comment)
                .HasColumnType("text");

            builder.HasIndex(r => r.ListingId);
            builder.HasIndex(r => new { r.ListingId, r.ReviewerId }).IsUnique();

            builder.HasOne<Listing>()
                .WithMany()
                .HasForeignKey(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
