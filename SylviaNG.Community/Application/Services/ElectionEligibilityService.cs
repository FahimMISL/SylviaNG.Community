using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Domain.Constants;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Services
{
    public class ElectionEligibilityService : IElectionEligibilityService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly ITeamRepository _teamRepository;

        public ElectionEligibilityService(
            IEmployeeRepository employeeRepository,
            ITeamMemberRepository teamMemberRepository,
            ITeamRepository teamRepository)
        {
            _employeeRepository = employeeRepository;
            _teamMemberRepository = teamMemberRepository;
            _teamRepository = teamRepository;
        }

        public async Task<HashSet<long>> GetEligibleEmployeeIdsAsync(Election election, List<ElectionAudienceTarget> targets)
        {
            var targetIds = ParseTargetIds(targets);
            return await ResolveEmployeeIdsAsync(election.AudienceScope, targetIds);
        }

        public async Task<HashSet<long>> ResolveEmployeeIdsAsync(string scope, List<long> targetIds)
        {
            if (scope == ElectionAudienceScope.Organization)
            {
                return (await _employeeRepository.GetActiveIdsAsync()).ToHashSet();
            }

            if (targetIds.Count == 0)
                return new HashSet<long>();

            var resolvedIds = scope switch
            {
                ElectionAudienceScope.Branch => await _employeeRepository.GetActiveIdsBySiteIdsAsync(targetIds),
                ElectionAudienceScope.Department => await _employeeRepository.GetActiveIdsByDepartmentIdsAsync(targetIds),
                ElectionAudienceScope.Team => await _teamMemberRepository.GetActiveEmployeeIdsByTeamIdsAsync(targetIds),
                ElectionAudienceScope.SelectedEmployees => await _employeeRepository.FilterActiveIdsAsync(targetIds),
                _ => new List<long>()
            };

            return resolvedIds.ToHashSet();
        }

        public async Task<HashSet<long>> ResolveCandidateEmployeeIdsAsync(string scope, List<long> targetIds)
        {
            var ids = await ResolveEmployeeIdsAsync(scope, targetIds);

            if (scope == ElectionAudienceScope.Team && targetIds.Count > 0)
            {
                var supervisorIds = await _teamRepository.GetSupervisorIdsByTeamIdsAsync(targetIds);
                var activeSupervisorIds = await _employeeRepository.FilterActiveIdsAsync(supervisorIds);
                ids.UnionWith(activeSupervisorIds);
            }

            return ids;
        }

        private static List<long> ParseTargetIds(List<ElectionAudienceTarget> targets)
        {
            var ids = new List<long>();
            foreach (var target in targets)
            {
                if (long.TryParse(target.TargetId, out var id))
                    ids.Add(id);
            }
            return ids;
        }
    }
}
