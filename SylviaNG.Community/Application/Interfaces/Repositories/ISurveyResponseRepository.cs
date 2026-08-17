using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyResponseRepository : IRepository<SurveyResponse>
    {
        Task<bool> ExistsAsync(long surveyId, long employeeId);
        Task<PagedResult<SurveyResponse>> GetPaginatedBySurveyIdAsync(long surveyId, PagedRequest request);

        /// <summary>
        /// Unpaged fetch of every response for a survey, used for full result aggregation
        /// (PagedRequest.PageSize is capped at 100, too small to rely on for this).
        /// </summary>
        Task<List<SurveyResponse>> GetAllBySurveyIdAsync(long surveyId);
    }
}
