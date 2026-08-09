using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyQuestionAdd
{
    public class SurveyQuestionAddCommand : IRequest<long>
    {
        public long SurveyId { get; set; }
        public SurveyQuestionCreateRequest Request { get; set; }

        public SurveyQuestionAddCommand(long surveyId, SurveyQuestionCreateRequest request)
        {
            SurveyId = surveyId;
            Request = request;
        }
    }
}
