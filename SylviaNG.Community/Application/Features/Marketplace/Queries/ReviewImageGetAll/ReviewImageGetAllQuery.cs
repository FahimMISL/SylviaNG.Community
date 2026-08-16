using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ReviewImageGetAll
{
    public class ReviewImageGetAllQuery : IRequest<List<ReviewImageResponse>>
    {
        public long ReviewId { get; set; }

        public ReviewImageGetAllQuery(long reviewId)
        {
            ReviewId = reviewId;
        }
    }
}
