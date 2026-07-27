using MediatR;
using SylviaNG.Community.Application.Features.Interests.Models;

namespace SylviaNG.Community.Application.Features.Interests.Queries.InterestGetById
{
    public class InterestGetByIdQuery : IRequest<InterestResponse>
    {
        public long InterestId { get; set; }

        public InterestGetByIdQuery(long interestId)
        {
            InterestId = interestId;
        }
    }
}
