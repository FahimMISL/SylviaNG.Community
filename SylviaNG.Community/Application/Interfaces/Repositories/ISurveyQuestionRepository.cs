using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyQuestionRepository : IRepository<SurveyQuestion>
    {
        Task<List<SurveyQuestion>> GetBySurveyIdAsync(long surveyId);
    }
}
