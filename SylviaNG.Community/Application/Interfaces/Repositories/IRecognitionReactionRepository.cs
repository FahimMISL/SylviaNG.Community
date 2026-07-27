using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IRecognitionReactionRepository : IRepository<RecognitionReaction>
    {
        Task<bool> ExistsAsync(long recognitionId, long employeeId);
        Task<RecognitionReaction?> GetAsync(long recognitionId, long employeeId);
        Task<List<RecognitionReaction>> GetByRecognitionIdAsync(long recognitionId);
    }
}
