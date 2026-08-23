using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface ISurveyService
    {
        Task<long> CreateAsync(SurveyCreateRequest request);
        Task UpdateAsync(long surveyId, SurveyUpdateRequest request);
        Task PublishAsync(long surveyId);
        Task CloseAsync(long surveyId);
        Task DeleteAsync(long surveyId);
        Task<SurveyDetailResponse> GetByIdAsync(long surveyId);
        Task<PagedResult<SurveyDetailResponse>> GetPaginatedAsync(PagedRequest request);

        Task<long> AddQuestionAsync(long surveyId, SurveyQuestionCreateRequest request);
        Task UpdateQuestionAsync(long surveyId, long questionId, SurveyQuestionUpdateRequest request);
        Task DeleteQuestionAsync(long surveyId, long questionId);
        Task<List<SurveyQuestionResponse>> GetQuestionsAsync(long surveyId);

        Task<long> AddAudienceAsync(long surveyId, SurveyAudienceCreateRequest request);
        Task<List<SurveyAudienceResponse>> GetAudienceAsync(long surveyId);

        Task<long> SubmitResponseAsync(long surveyId, SurveySubmissionRequest request, long employeeId);
        Task<PagedResult<SurveySubmissionResponse>> GetResponsesAsync(long surveyId, PagedRequest request);
        Task<SurveyResultsResponse> GetResultsAsync(long surveyId);
    }
}
