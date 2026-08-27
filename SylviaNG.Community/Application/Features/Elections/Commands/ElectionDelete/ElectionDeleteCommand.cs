using MediatR;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionDelete
{
    public class ElectionDeleteCommand : IRequest
    {
        public long ElectionId { get; set; }

        public ElectionDeleteCommand(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
