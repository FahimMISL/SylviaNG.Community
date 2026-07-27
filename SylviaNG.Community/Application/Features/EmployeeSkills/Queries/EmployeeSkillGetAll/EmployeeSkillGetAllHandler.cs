using MediatR;
using SylviaNG.Community.Application.Features.EmployeeSkills.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.EmployeeSkills.Queries.EmployeeSkillGetAll
{
    public class EmployeeSkillGetAllHandler : IRequestHandler<EmployeeSkillGetAllQuery, List<EmployeeSkillResponse>>
    {
        private readonly ISkillService _skillService;

        public EmployeeSkillGetAllHandler(ISkillService skillService)
        {
            _skillService = skillService;
        }

        public async Task<List<EmployeeSkillResponse>> Handle(EmployeeSkillGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _skillService.GetEmployeeSkillsAsync(query.EmployeeId);
        }
    }
}
