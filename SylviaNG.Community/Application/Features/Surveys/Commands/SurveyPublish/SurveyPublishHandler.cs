using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyPublish
{
    public class SurveyPublishHandler : IRequestHandler<SurveyPublishCommand>
    {
        private readonly ISurveyService _surveyService;

        public SurveyPublishHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task Handle(SurveyPublishCommand command, CancellationToken cancellationToken)
        {
            await _surveyService.PublishAsync(command.SurveyId);
        }
    }
}
