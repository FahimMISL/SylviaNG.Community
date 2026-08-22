using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit
{
    public class SurveyResponseSubmitHandler : IRequestHandler<SurveyResponseSubmitCommand, long>
    {
        private readonly ISurveyService _surveyService;

        public SurveyResponseSubmitHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task<long> Handle(SurveyResponseSubmitCommand command, CancellationToken cancellationToken)
        {
            return await _surveyService.SubmitResponseAsync(command.SurveyId, command.Request, command.EmployeeId);
        }
    }
}
