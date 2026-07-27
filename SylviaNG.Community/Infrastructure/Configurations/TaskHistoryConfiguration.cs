using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class TaskHistoryConfiguration : IEntityTypeConfiguration<TaskHistory>
    {
        public void Configure(EntityTypeBuilder<TaskHistory> builder)
        {
            builder.ToTable("TaskHistories");
            builder.HasKey(h => h.HistoryId);

            builder.Property(h => h.Action)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.OldValue)
                .HasColumnType("text");

            builder.Property(h => h.NewValue)
                .HasColumnType("text");

            builder.HasIndex(h => h.TaskId);
            builder.HasIndex(h => h.ChangedBy);

            builder.HasOne<TaskEntity>()
                .WithMany()
                .HasForeignKey(h => h.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
