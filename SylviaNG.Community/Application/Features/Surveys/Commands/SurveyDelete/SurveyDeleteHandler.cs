using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyDelete
{
    public class SurveyDeleteHandler : IRequestHandler<SurveyDeleteCommand>
    {
        private readonly ISurveyService _surveyService;

        public SurveyDeleteHandler(ISurveyService surveyService)
        {
            _surveyService = surveyService;
        }

        public async Task Handle(SurveyDeleteCommand command, CancellationToken cancellationToken)
        {
            await _surveyService.DeleteAsync(command.SurveyId);
        }
    }
}
