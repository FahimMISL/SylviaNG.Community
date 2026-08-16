using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
    {
        public void Configure(EntityTypeBuilder<Purchase> builder)
        {
            builder.ToTable("Purchases");
            builder.HasKey(p => p.PurchaseId);

            builder.Property(p => p.UnitPrice)
                .HasPrecision(18, 2);

            builder.Property(p => p.Currency)
                .IsRequired()
                .HasMaxLength(10);

            builder.HasIndex(p => p.ListingId);
            builder.HasIndex(p => p.BuyerId);

            builder.HasOne<Listing>()
                .WithMany()
                .HasForeignKey(p => p.ListingId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
