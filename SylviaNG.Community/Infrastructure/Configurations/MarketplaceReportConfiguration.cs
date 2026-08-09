using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class MarketplaceReportConfiguration : IEntityTypeConfiguration<MarketplaceReport>
    {
        public void Configure(EntityTypeBuilder<MarketplaceReport> builder)
        {
            builder.ToTable("MarketplaceReports");
            builder.HasKey(r => r.ReportId);

            builder.Property(r => r.Reason)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(r => r.Status)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(r => r.ListingId);
            builder.HasIndex(r => r.ReportedBy);
            builder.HasIndex(r => r.Status);

            builder.HasOne<Listing>()
                .WithMany()
                .HasForeignKey(r => r.ListingId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
