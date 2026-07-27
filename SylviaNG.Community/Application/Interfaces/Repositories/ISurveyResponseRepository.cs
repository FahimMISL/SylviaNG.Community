using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyResponseRepository : IRepository<SurveyResponse>
    {
        Task<bool> ExistsAsync(long surveyId, long employeeId);
        Task<PagedResult<SurveyResponse>> GetPaginatedBySurveyIdAsync(long surveyId, PagedRequest request);
    }
}
