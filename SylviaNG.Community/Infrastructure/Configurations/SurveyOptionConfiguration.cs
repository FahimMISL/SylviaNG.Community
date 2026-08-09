using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class SurveyOptionConfiguration : IEntityTypeConfiguration<SurveyOption>
    {
        public void Configure(EntityTypeBuilder<SurveyOption> builder)
        {
            builder.ToTable("SurveyOptions");
            builder.HasKey(o => o.OptionId);

            builder.Property(o => o.OptionText)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(o => o.QuestionId);
            builder.HasIndex(o => new { o.QuestionId, o.DisplayOrder });

            builder.HasOne<SurveyQuestion>()
                .WithMany()
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
