using MediatR;

namespace SylviaNG.Community.Application.Features.Elections.Commands.ElectionClose
{
    public class ElectionCloseCommand : IRequest
    {
        public long ElectionId { get; set; }

        public ElectionCloseCommand(long electionId)
        {
            ElectionId = electionId;
        }
    }
}
