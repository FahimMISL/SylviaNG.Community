using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ListingImageConfiguration : IEntityTypeConfiguration<ListingImage>
    {
        public void Configure(EntityTypeBuilder<ListingImage> builder)
        {
            builder.ToTable("ListingImages");
            builder.HasKey(li => li.ImageId);

            builder.Property(li => li.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(li => li.ListingId);

            builder.HasOne<Listing>()
                .WithMany()
                .HasForeignKey(li => li.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
