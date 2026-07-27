using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyAnswerRepository : IRepository<SurveyAnswer>
    {
        Task<List<SurveyAnswer>> GetByResponseIdAsync(long responseId);
        Task<List<SurveyAnswer>> GetByResponseIdsAsync(IEnumerable<long> responseIds);
    }
}
