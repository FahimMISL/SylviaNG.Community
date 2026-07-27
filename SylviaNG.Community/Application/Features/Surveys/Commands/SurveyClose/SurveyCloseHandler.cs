using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyClose
{
    public class SurveyCloseHandler : IRequestHandler<SurveyCloseCommand>
    {
        private readonly ISurveyService _surveyService;

        public SurveyCloseHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task Handle(SurveyCloseCommand command, CancellationToken cancellationToken)
        {
            await _surveyService.CloseAsync(command.SurveyId);
        }
    }
}
