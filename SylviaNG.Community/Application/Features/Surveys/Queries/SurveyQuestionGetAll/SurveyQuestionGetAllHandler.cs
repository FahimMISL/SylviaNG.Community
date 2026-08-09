using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyQuestionGetAll
{
    public class SurveyQuestionGetAllHandler : IRequestHandler<SurveyQuestionGetAllQuery, List<SurveyQuestionResponse>>
    {
        private readonly ISurveyService _surveyService;

        public SurveyQuestionGetAllHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<List<SurveyQuestionResponse>> Handle(SurveyQuestionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _surveyService.GetQuestionsAsync(query.SurveyId);
        }
    }
}
