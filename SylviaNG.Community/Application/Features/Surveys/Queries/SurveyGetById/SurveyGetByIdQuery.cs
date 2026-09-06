using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetById
{
    public class SurveyGetByIdQuery : IRequest<SurveyDetailResponse>
    {
        public long SurveyId { get; set; }
        public bool IsHrOrAdmin { get; set; }
        public long? EmployeeId { get; set; }

        public SurveyGetByIdQuery(long surveyId, bool isHrOrAdmin, long? employeeId)
        {
            SurveyId = surveyId;
            IsHrOrAdmin = isHrOrAdmin;
            EmployeeId = employeeId;
        }
    }
}
