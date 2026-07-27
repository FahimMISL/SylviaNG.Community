using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class RecognitionCommentConfiguration : IEntityTypeConfiguration<RecognitionComment>
    {
        public void Configure(EntityTypeBuilder<RecognitionComment> builder)
        {
            builder.ToTable("RecognitionComments");
            builder.HasKey(rc => rc.CommentId);

            builder.Property(rc => rc.Comment)
                .IsRequired()
                .HasColumnType("text");

            builder.HasIndex(rc => rc.RecognitionId);
            builder.HasIndex(rc => rc.ParentCommentId);

            builder.HasOne<Recognition>()
                .WithMany()
                .HasForeignKey(rc => rc.RecognitionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<RecognitionComment>()
                .WithMany()
                .HasForeignKey(rc => rc.ParentCommentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
