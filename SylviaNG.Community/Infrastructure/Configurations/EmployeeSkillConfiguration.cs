using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class EmployeeSkillConfiguration : IEntityTypeConfiguration<EmployeeSkill>
    {
        public void Configure(EntityTypeBuilder<EmployeeSkill> builder)
        {
            builder.ToTable("EmployeeSkills");
            builder.HasKey(es => es.EmployeeSkillId);

            builder.HasIndex(es => new { es.EmployeeId, es.SkillId }).IsUnique();
            builder.HasIndex(es => es.EmployeeId);

            builder.HasOne<Skill>()
                .WithMany()
                .HasForeignKey(es => es.SkillId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
