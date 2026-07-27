using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyUpdate
{
    public class SurveyUpdateHandler : IRequestHandler<SurveyUpdateCommand>
    {
        private readonly ISurveyService _surveyService;

        public SurveyUpdateHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task Handle(SurveyUpdateCommand command, CancellationToken cancellationToken)
        {
            await _surveyService.UpdateAsync(command.SurveyId, command.Request);
        }
    }
}
