using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class RecurringTaskConfiguration : IEntityTypeConfiguration<RecurringTask>
    {
        public void Configure(EntityTypeBuilder<RecurringTask> builder)
        {
            builder.ToTable("RecurringTasks");
            builder.HasKey(rt => rt.RecurringTaskId);

            builder.Property(rt => rt.Frequency)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(rt => rt.IsActive);
        }
    }
}
