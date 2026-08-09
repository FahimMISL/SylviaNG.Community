using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit
{
    public class SurveyResponseSubmitCommand : IRequest<long>
    {
        public long SurveyId { get; set; }
        public SurveySubmissionRequest Request { get; set; }

        public SurveyResponseSubmitCommand(long surveyId, SurveySubmissionRequest request)
        {
            SurveyId = surveyId;
            Request = request;
        }
    }
}
