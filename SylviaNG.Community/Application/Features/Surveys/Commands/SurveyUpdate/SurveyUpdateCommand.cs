using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyUpdate
{
    public class SurveyUpdateCommand : IRequest
    {
        public long SurveyId { get; set; }
        public SurveyUpdateRequest Request { get; set; }

        public SurveyUpdateCommand(long surveyId, SurveyUpdateRequest request)
        {
            SurveyId = surveyId;
            Request = request;
        }
    }
}
