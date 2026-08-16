using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ReviewImageConfiguration : IEntityTypeConfiguration<ReviewImage>
    {
        public void Configure(EntityTypeBuilder<ReviewImage> builder)
        {
            builder.ToTable("ReviewImages");
            builder.HasKey(ri => ri.ImageId);

            builder.Property(ri => ri.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(ri => ri.ReviewId);

            builder.HasOne<Review>()
                .WithMany()
                .HasForeignKey(ri => ri.ReviewId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
