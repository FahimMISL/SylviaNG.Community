using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            builder.ToTable("Listings");
            builder.HasKey(l => l.ListingId);

            builder.Property(l => l.ListingType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(l => l.Description)
                .HasColumnType("text");

            builder.Property(l => l.Category)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(l => l.Condition)
                .HasMaxLength(50);

            builder.Property(l => l.Price)
                .HasPrecision(18, 2);

            builder.Property(l => l.Currency)
                .IsRequired()
                .HasMaxLength(10);

            builder.Property(l => l.Location)
                .HasMaxLength(200);

            builder.Property(l => l.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.ApprovalStatus)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(l => l.RejectionReason)
                .HasColumnType("text");

            builder.HasIndex(l => l.SellerId);
            builder.HasIndex(l => l.Category);
            builder.HasIndex(l => l.Status);
            builder.HasIndex(l => l.ApprovalStatus);
        }
    }
}
