using MediatR;
using SylviaNG.Community.Application.Features.FileStorages.Models;

namespace SylviaNG.Community.Application.Features.FileStorages.Commands.FileStorageCreate
{
    public class FileStorageCreateCommand : IRequest<long>
    {
        public FileStorageCreateRequest Request { get; set; }

        public FileStorageCreateCommand(FileStorageCreateRequest request)
        {
            Request = request;
        }
    }
}
