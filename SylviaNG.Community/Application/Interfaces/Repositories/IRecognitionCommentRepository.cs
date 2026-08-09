using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface IRecognitionCommentRepository : IRepository<RecognitionComment>
    {
        Task<List<RecognitionComment>> GetByRecognitionIdAsync(long recognitionId);
    }
}
