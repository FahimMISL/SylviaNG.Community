using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class SurveyResponseConfiguration : IEntityTypeConfiguration<SurveyResponse>
    {
        public void Configure(EntityTypeBuilder<SurveyResponse> builder)
        {
            builder.ToTable("SurveyResponses");
            builder.HasKey(r => r.ResponseId);

            builder.Property(r => r.CompletionStatus)
                .IsRequired()
                .HasMaxLength(50);

            // One response per employee per survey.
            builder.HasIndex(r => new { r.SurveyId, r.EmployeeId }).IsUnique();
            builder.HasIndex(r => r.EmployeeId);

            builder.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(r => r.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<Employee>()
                .WithMany()
                .HasForeignKey(r => r.EmployeeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
