using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewCreate
{
    public class ReviewCreateCommand : IRequest<long>
    {
        public long ReviewerId { get; set; }
        public ReviewCreateRequest Request { get; set; }

        public ReviewCreateCommand(long reviewerId, ReviewCreateRequest request)
        {
            ReviewerId = reviewerId;
            Request = request;
        }
    }
}
