using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyAudienceRepository : IRepository<SurveyAudience>
    {
        Task<List<SurveyAudience>> GetBySurveyIdAsync(long surveyId);
    }
}
