using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionUpdate
{
    public class SurveyQuestionUpdateHandler : IRequestHandler<SurveyQuestionUpdateCommand>
    {
        private readonly ISurveyService _surveyService;

        public SurveyQuestionUpdateHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task Handle(SurveyQuestionUpdateCommand command, CancellationToken cancellationToken)
        {
            await _surveyService.UpdateQuestionAsync(command.SurveyId, command.QuestionId, command.Request);
        }
    }
}
