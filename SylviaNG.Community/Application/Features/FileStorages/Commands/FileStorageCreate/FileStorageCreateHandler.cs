using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate
{
    public class FileStorageCreateHandler : IRequestHandler<FileStorageCreateCommand, long>
    {
        private readonly IFileStorageService _fileStorageService;

        public FileStorageCreateHandler(IFileStorageService fileStorageService)
        {
            _fileStorageService = fileStorageService;
        }

        public async Task<long> Handle(FileStorageCreateCommand command, CancellationToken cancellationToken)
        {
            return await _fileStorageService.CreateAsync(command.Request);
        }
    }
}
