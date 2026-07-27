using MediatR;
using SylviaNG.Community.Application.Features.Elections.Models;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionAudienceTargetAdd
{
    public class ElectionAudienceTargetAddCommand : IRequest<long>
    {
        public long ElectionId { get; set; }
        public ElectionAudienceTargetAddRequest Request { get; set; }

        public ElectionAudienceTargetAddCommand(long electionId, ElectionAudienceTargetAddRequest request)
        {
            ElectionId = electionId;
            Request = request;
        }
    }
}
