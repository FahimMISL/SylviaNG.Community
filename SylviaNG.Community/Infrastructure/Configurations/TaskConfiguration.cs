using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.ToTable("Tasks");
            builder.HasKey(t => t.TaskId);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.Description)
                .HasColumnType("text");

            builder.Property(t => t.Priority)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(t => t.TaskStatus)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnName("Status");

            // Indexes to support filtering by status/assignee/team (see ITaskRepository.GetPaginatedAsync).
            builder.HasIndex(t => t.TeamId);
            builder.HasIndex(t => t.AssignedBy);
            builder.HasIndex(t => t.AssignedTo);
            builder.HasIndex(t => t.RecurringTaskId);
            builder.HasIndex(t => t.TaskStatus);
            builder.HasIndex(t => t.Priority);
            builder.HasIndex(t => t.DueDate);

            // Team is the owning aggregate for team-assigned tasks; null TeamId means an individual
            // (non-team) task (US-7.6/7.7), so the FK is optional. SetNull (not Cascade) because a
            // nullable FK can't cascade-delete - deleting the team detaches its tasks instead of
            // deleting them.
            builder.HasOne<Team>()
                .WithMany()
                .HasForeignKey(t => t.TeamId)
                .IsRequired(false)
                .OnDelete(DeleteBehavior.SetNull);

            // Optional link back to the recurrence definition that generated this task.
            // Restrict (not Cascade/SetNull) to avoid multi-path cascade-delete cycles on
            // providers that disallow them (SQL Server) - mirrors PostComment's self-referencing FK.
            builder.HasOne<RecurringTask>()
                .WithMany()
                .HasForeignKey(t => t.RecurringTaskId)
                .OnDelete(DeleteBehavior.Restrict);

            // AssignedBy/AssignedTo reference Employee, which is synced via Kafka and not
            // given an enforced FK constraint anywhere else in this codebase (see
            // TeamMemberConfiguration/PostCommentConfiguration) - indexed only.
        }
    }
}
