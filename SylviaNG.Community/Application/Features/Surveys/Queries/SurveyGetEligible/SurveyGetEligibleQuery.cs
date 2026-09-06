using MediatR;
using SylviaNG.Community.Application.Features.Surveys.Models;

namespace SylviaNG.Community.Application.Features.Surveys.Queries.SurveyGetEligible
{
    public class SurveyGetEligibleQuery : IRequest<List<SurveyDetailResponse>>
    {
        public long EmployeeId { get; set; }

        public SurveyGetEligibleQuery(long employeeId)
        {
            EmployeeId = employeeId;
        }
    }
}
