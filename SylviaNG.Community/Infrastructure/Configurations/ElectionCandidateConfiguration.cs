using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class ElectionCandidateConfiguration : IEntityTypeConfiguration<ElectionCandidate>
    {
        public void Configure(EntityTypeBuilder<ElectionCandidate> builder)
        {
            builder.ToTable("ElectionCandidates");
            builder.HasKey(c => c.ElectionCandidateId);

            builder.Property(c => c.CandidateType)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(c => c.Manifesto)
                .HasColumnType("text");

            builder.HasIndex(c => c.ElectionId);
            builder.HasIndex(c => c.EmployeeId);
            builder.HasIndex(c => c.TeamId);

            builder.HasOne<Election>()
                .WithMany()
                .HasForeignKey(c => c.ElectionId)
                .OnDelete(DeleteBehavior.Cascade);

            // Employee is treated as an externally-synced entity (see EmployeeEventConsumer) -
            // like TeamMember.EmployeeId elsewhere in this codebase, it is indexed but not a
            // DB-enforced FK. Team is owned within this bounded context, so it gets a real FK;
            // Restrict (not Cascade) is used here to avoid deleting nomination history if a
            // Team is ever removed, and to avoid a second cascade path into this table
            // alongside the Election FK above.
            builder.HasOne<Team>()
                .WithMany()
                .HasForeignKey(c => c.TeamId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
