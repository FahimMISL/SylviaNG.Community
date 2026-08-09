using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class SurveyAudienceConfiguration : IEntityTypeConfiguration<SurveyAudience>
    {
        public void Configure(EntityTypeBuilder<SurveyAudience> builder)
        {
            builder.ToTable("SurveyAudiences");
            builder.HasKey(a => a.AudienceId);

            builder.Property(a => a.AudienceType)
                .IsRequired()
                .HasMaxLength(50);

            // DepartmentId/BranchId are raw IDs resolved via gRPC to the Core microservice -
            // intentionally no FK constraint or navigation property.
            builder.HasIndex(a => a.SurveyId);
            builder.HasIndex(a => a.DepartmentId);
            builder.HasIndex(a => a.BranchId);

            builder.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(a => a.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
