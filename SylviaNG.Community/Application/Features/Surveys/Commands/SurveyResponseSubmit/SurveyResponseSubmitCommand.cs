using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Commands.SurveyResponseSubmit
{
    public class SurveyResponseSubmitCommand : IRequest<long>
    {
        public long SurveyId { get; set; }
        public SurveySubmissionRequest Request { get; set; }

        /// <summary>
        /// Resolved server-side by SurveyController from ICurrentUserService, never taken
        /// from the client-supplied request body - see SurveyController.SubmitResponse.
        /// </summary>
        public long EmployeeId { get; set; }

        public SurveyResponseSubmitCommand(long surveyId, SurveySubmissionRequest request, long employeeId)
        {
            SurveyId = surveyId;
            Request = request;
            EmployeeId = employeeId;
        }
    }
}
