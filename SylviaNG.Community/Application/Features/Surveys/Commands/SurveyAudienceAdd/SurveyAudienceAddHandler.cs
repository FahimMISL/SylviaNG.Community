using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyAudienceAdd
{
    public class SurveyAudienceAddHandler : IRequestHandler<SurveyAudienceAddCommand, long>
    {
        private readonly ISurveyService _surveyService;

        public SurveyAudienceAddHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<long> Handle(SurveyAudienceAddCommand command, CancellationToken cancellationToken)
        {
            return await _surveyService.AddAudienceAsync(command.SurveyId, command.Request);
        }
    }
}
