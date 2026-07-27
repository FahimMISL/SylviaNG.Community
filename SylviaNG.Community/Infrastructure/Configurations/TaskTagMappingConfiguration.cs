using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;
using TaskEntity = SylviaNG.Community.Domain.Entities.Task;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class TaskTagMappingConfiguration : IEntityTypeConfiguration<TaskTagMapping>
    {
        public void Configure(EntityTypeBuilder<TaskTagMapping> builder)
        {
            builder.ToTable("TaskTagMappings");
            builder.HasKey(m => m.MappingId);

            builder.HasIndex(m => new { m.TaskId, m.TagId }).IsUnique();
            builder.HasIndex(m => m.TagId);

            builder.HasOne<TaskEntity>()
                .WithMany()
                .HasForeignKey(m => m.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<TaskTag>()
                .WithMany()
                .HasForeignKey(m => m.TagId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
