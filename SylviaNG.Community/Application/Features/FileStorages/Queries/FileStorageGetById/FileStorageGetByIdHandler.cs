using MediatR;
using SylviaNG.Community.Application.Features.FileStorages.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.FileStorages.Queries.FileStorageGetById
{
    public class FileStorageGetByIdHandler : IRequestHandler<FileStorageGetByIdQuery, FileStorageResponse>
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageGetByIdHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<FileStorageResponse> Handle(FileStorageGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _fileStorageService.GetByIdAsync(query.FileId);
        }
    }
}
