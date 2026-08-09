using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Infrastructure.Configurations
{
    public class FileStorageConfiguration : IEntityTypeConfiguration<FileStorage>
    {
        public void Configure(EntityTypeBuilder<FileStorage> builder)
        {
            builder.ToTable("FileStorages");
            builder.HasKey(f => f.FileId);

            builder.Property(f => f.Module)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(f => f.FileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.OriginalFileName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(f => f.FileExtension)
                .HasMaxLength(20);

            builder.Property(f => f.MimeType)
                .HasMaxLength(100);

            builder.Property(f => f.StoragePath)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasIndex(f => f.Module);
            builder.HasIndex(f => f.EntityId);
            builder.HasIndex(f => f.UploadedBy);
        }
    }
}
