using MediatR;
using SylviaNG.Community.Application.Features.Skills.Models;

namespace SylviaNG.Community.Application.Features.Skills.Queries.SkillGetAll
{
    public class SkillGetAllQuery : IRequest<List<SkillResponse>>
    {
    }
}
