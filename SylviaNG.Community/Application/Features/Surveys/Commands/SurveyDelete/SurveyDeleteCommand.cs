using MediatR;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyDelete
{
    public class SurveyDeleteCommand : IRequest
    {
        public long SurveyId { get; set; }

        public SurveyDeleteCommand(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
