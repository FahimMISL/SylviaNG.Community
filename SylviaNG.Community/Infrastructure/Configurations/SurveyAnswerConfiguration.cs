using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class SurveyAnswerConfiguration : IEntityTypeConfiguration<SurveyAnswer>
    {
        public void Configure(EntityTypeBuilder<SurveyAnswer> builder)
        {
            builder.ToTable("SurveyAnswers");
            builder.HasKey(a => a.AnswerId);

            builder.Property(a => a.AnswerText)
                .HasColumnType("text");

            builder.HasIndex(a => a.ResponseId);
            builder.HasIndex(a => a.QuestionId);

            // ResponseId cascades from its parent SurveyResponse. QuestionId/OptionId use
            // Restrict to avoid multiple cascade paths (SQL Server rejects cycles/multiple
            // cascade paths on a single table when more than one FK would cascade-delete it).
            builder.HasOne<SurveyResponse>()
                .WithMany()
                .HasForeignKey(a => a.ResponseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<SurveyQuestion>()
                .WithMany()
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne<SurveyOption>()
                .WithMany()
                .HasForeignKey(a => a.OptionId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
