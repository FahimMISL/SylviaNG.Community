using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyQuestionGetAll
{
    public class SurveyQuestionGetAllQuery : IRequest<List<SurveyQuestionResponse>>
    {
        public long SurveyId { get; set; }
        public bool IsHrOrAdmin { get; set; }
        public long? EmployeeId { get; set; }

        public SurveyQuestionGetAllQuery(long surveyId, bool isHrOrAdmin, long? employeeId)
        {
            SurveyId = surveyId;
            IsHrOrAdmin = isHrOrAdmin;
            EmployeeId = employeeId;
        }
    }
}
