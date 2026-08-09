using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class FileStorageMapper
    {
        public static FileStorage ToEntity(this FileStorageCreateRequest request)
        {
            return new FileStorage
            {
                Module = request.Module,
                EntityId = request.EntityId,
                FileName = request.FileName,
                OriginalFileName = request.OriginalFileName,
                FileExtension = request.FileExtension,
                MimeType = request.MimeType,
                FileSize = request.FileSize,
                StoragePath = request.StoragePath,
                UploadedBy = request.UploadedBy
            };
        }

        public static FileStorageResponse ToResponse(this FileStorage entity)
        {
            return new FileStorageResponse
            {
                FileId = entity.FileId,
                Module = entity.Module,
                EntityId = entity.EntityId,
                FileName = entity.FileName,
                OriginalFileName = entity.OriginalFileName,
                FileExtension = entity.FileExtension,
                MimeType = entity.MimeType,
                FileSize = entity.FileSize,
                StoragePath = entity.StoragePath,
                UploadedBy = entity.UploadedBy,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
