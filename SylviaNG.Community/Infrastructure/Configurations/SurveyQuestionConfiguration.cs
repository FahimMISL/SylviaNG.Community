using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class SurveyQuestionConfiguration : IEntityTypeConfiguration<SurveyQuestion>
    {
        public void Configure(EntityTypeBuilder<SurveyQuestion> builder)
        {
            builder.ToTable("SurveyQuestions");
            builder.HasKey(q => q.QuestionId);

            builder.Property(q => q.QuestionText)
                .IsRequired()
                .HasColumnType("text");

            builder.Property(q => q.QuestionType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(q => q.SurveyId);
            builder.HasIndex(q => new { q.SurveyId, q.DisplayOrder });

            builder.HasOne<Survey>()
                .WithMany()
                .HasForeignKey(q => q.SurveyId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
