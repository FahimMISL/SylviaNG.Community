using MediatR;
using SylviaNG.Community.Application.Features.FileStorages.Models;

namespace SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetById
{
    public class FileStorageGetByIdQuery : IRequest<FileStorageResponse>
    {
        public long FileId { get; set; }

        public FileStorageGetByIdQuery(long fileId)
        {
            FileId = fileId;
        }
    }
}
