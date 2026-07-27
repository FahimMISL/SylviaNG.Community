using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyOptionRepository : IRepository<SurveyOption>
    {
        Task<List<SurveyOption>> GetByQuestionIdAsync(long questionId);
        Task<List<SurveyOption>> GetByQuestionIdsAsync(IEnumerable<long> questionIds);
    }
}
