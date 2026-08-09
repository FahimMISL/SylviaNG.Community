using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyCreate
{
    public class SurveyCreateHandler : IRequestHandler<SurveyCreateCommand, long>
    {
        private readonly ISurveyService _surveyService;

        public SurveyCreateHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<long> Handle(SurveyCreateCommand command, CancellationToken cancellationToken)
        {
            return await _surveyService.CreateAsync(command.Request);
        }
    }
}
