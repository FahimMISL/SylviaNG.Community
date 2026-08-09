using MediatR;

namespace SylviaNG.Community.Application.Features.Interests.Commands.InterestDelete
{
    public class InterestDeleteCommand : IRequest
    {
        public long InterestId { get; set; }

        public InterestDeleteCommand(long interestId)
        {
            InterestId = interestId;
        }
    }
}
