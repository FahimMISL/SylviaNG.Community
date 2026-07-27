using SylviaNG.Community.Application.Features.EmployeeSkills.Models;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class SkillMapper
    {
        public static Skill ToEntity(this SkillCreateRequest request)
        {
            return new Skill
            {
                Name = request.Name
            };
        }

        public static SkillResponse ToResponse(this Skill entity)
        {
            return new SkillResponse
            {
                SkillId = entity.SkillId,
                Name = entity.Name
            };
        }

        public static EmployeeSkill ToEntity(this EmployeeSkillAssignRequest request, long employeeId)
        {
            return new EmployeeSkill
            {
                EmployeeId = employeeId,
                SkillId = request.SkillId
            };
        }

        public static EmployeeSkillResponse ToResponse(this EmployeeSkill entity)
        {
            return new EmployeeSkillResponse
            {
                EmployeeSkillId = entity.EmployeeSkillId,
                EmployeeId = entity.EmployeeId,
                SkillId = entity.SkillId
            };
        }
    }
}
