using MediatR;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionPublish
{
    public class ElectionPublishCommand : IRequest
    {
        public long ElectionId { get; set; }

        public ElectionPublishCommand(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
