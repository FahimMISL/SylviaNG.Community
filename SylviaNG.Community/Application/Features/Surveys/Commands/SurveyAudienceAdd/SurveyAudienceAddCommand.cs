using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyAudienceAdd
{
    public class SurveyAudienceAddCommand : IRequest<long>
    {
        public long SurveyId { get; set; }
        public SurveyAudienceCreateRequest Request { get; set; }

        public SurveyAudienceAddCommand(long surveyId, SurveyAudienceCreateRequest request)
        {
            SurveyId = surveyId;
            Request = request;
        }
    }
}
