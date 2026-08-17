using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class GroupMemberConfiguration : IEntityTypeConfiguration<GroupMember>
    {
        public void Configure(EntityTypeBuilder<GroupMember> builder)
        {
            builder.ToTable("GroupMembers");
            builder.HasKey(gm => gm.GroupMemberId);

            builder.Property(gm => gm.Role)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20);

            builder.HasIndex(gm => new { gm.GroupId, gm.EmployeeId }).IsUnique();
            builder.HasIndex(gm => gm.EmployeeId);
            builder.HasIndex(gm => gm.Role);

            builder.HasOne<Group>()
                .WithMany()
                .HasForeignKey(gm => gm.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
