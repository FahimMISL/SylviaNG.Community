using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ReviewImageAdd
{
    public class ReviewImageAddCommand : IRequest<long>
    {
        public long ReviewId { get; set; }
        public ReviewImageAddRequest Request { get; set; }

        public ReviewImageAddCommand(long reviewId, ReviewImageAddRequest request)
        {
            ReviewId = reviewId;
            Request = request;
        }
    }
}
