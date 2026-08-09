using MediatR;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyClose
{
    public class SurveyCloseCommand : IRequest
    {
        public long SurveyId { get; set; }

        public SurveyCloseCommand(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
