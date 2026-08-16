using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IReviewImageRepository : IRepository<ReviewImage>
    {
        Task<List<ReviewImage>> GetByReviewIdAsync(long reviewId);
    }
}
