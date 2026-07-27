using MediatR;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetAllPaged
{
    public class FileStorageGetAllPagedHandler : IRequestHandler<FileStorageGetAllPagedQuery, PagedResult<FileStorageResponse>>
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageGetAllPagedHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<PagedResult<FileStorageResponse>> Handle(FileStorageGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _fileStorageService.GetPaginatedAsync(query.Request, query.Module, query.EntityId);
        }
    }
}
