using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionCreate
{
    public class ElectionCreateCommand : IRequest<long>
    {
        public ElectionCreateRequest Request { get; set; }
        public long? CreatedBy { get; set; }

        public ElectionCreateCommand(ElectionCreateRequest request, long? createdBy)
        {
            Request = request;
            CreatedBy = createdBy;
        }
    }
}
