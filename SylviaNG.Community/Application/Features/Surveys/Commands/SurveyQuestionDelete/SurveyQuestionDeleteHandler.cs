using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionDelete
{
    public class SurveyQuestionDeleteHandler : IRequestHandler<SurveyQuestionDeleteCommand>
    {
        private readonly ISurveyService _surveyService;

        public SurveyQuestionDeleteHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task Handle(SurveyQuestionDeleteCommand command, CancellationToken cancellationToken)
        {
            await _surveyService.DeleteQuestionAsync(command.SurveyId, command.QuestionId);
        }
    }
}
