using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionUpdate
{
    public class ElectionUpdateCommand : IRequest
    {
        public long ElectionId { get; set; }
        public ElectionUpdateRequest Request { get; set; }

        public ElectionUpdateCommand(long electionId, ElectionUpdateRequest request)
        {
            ElectionId = electionId;
            Request = request;
        }
    }
}
