using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IRecognitionBadgeRepository : IRepository<RecognitionBadge>
    {
        Task<List<RecognitionBadge>> GetByRecognitionIdAsync(long recognitionId);
        Task<List<RecognitionBadge>> GetByRecognitionIdsAsync(IEnumerable<long> recognitionIds);
    }
}
