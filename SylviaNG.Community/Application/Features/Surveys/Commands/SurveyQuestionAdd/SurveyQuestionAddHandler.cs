using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionAdd
{
    public class SurveyQuestionAddHandler : IRequestHandler<SurveyQuestionAddCommand, long>
    {
        private readonly ISurveyService _surveyService;

        public SurveyQuestionAddHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<long> Handle(SurveyQuestionAddCommand command, CancellationToken cancellationToken)
        {
            return await _surveyService.AddQuestionAsync(command.SurveyId, command.Request);
        }
    }
}
