using MediatR;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetAllPaged
{
    public class FileStorageGetAllPagedQuery : IRequest<PagedResult<FileStorageResponse>>
    {
        public PagedRequest Request { get; set; }
        public string? Module { get; set; }
        public long? EntityId { get; set; }

        public FileStorageGetAllPagedQuery(PagedRequest request, string? module, long? entityId)
        {
            Request = request;
            Module = module;
            EntityId = entityId;
        }
    }
}
