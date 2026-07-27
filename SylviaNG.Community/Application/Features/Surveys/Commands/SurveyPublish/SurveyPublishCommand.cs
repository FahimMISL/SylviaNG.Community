using MediatR;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyPublish
{
    public class SurveyPublishCommand : IRequest
    {
        public long SurveyId { get; set; }

        public SurveyPublishCommand(long surveyId)
        {
            SurveyId = surveyId;
        }
    }
}
