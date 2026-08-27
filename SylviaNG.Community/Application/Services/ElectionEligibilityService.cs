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

        public ElectionEligibilityService(
            IEmployeeRepository employeeRepository,
            ITeamMemberRepository teamMemberRepository)
        {
            _employeeRepository = employeeRepository;
            _teamMemberRepository = teamMemberRepository;
        }

        public async Task<HashSet<long>> GetEligibleEmployeeIdsAsync(Election election, List<ElectionAudienceTarget> targets)
        {
            if (election.AudienceScope == ElectionAudienceScope.Organization)
            {
                return (await _employeeRepository.GetActiveIdsAsync()).ToHashSet();
            }

            var targetIds = ParseTargetIds(targets);
            if (targetIds.Count == 0)
                return new HashSet<long>();

            var eligibleIds = election.AudienceScope switch
            {
                ElectionAudienceScope.Branch => await _employeeRepository.GetActiveIdsBySiteIdsAsync(targetIds),
                ElectionAudienceScope.Department => await _employeeRepository.GetActiveIdsByDepartmentIdsAsync(targetIds),
                ElectionAudienceScope.Team => await _teamMemberRepository.GetActiveEmployeeIdsByTeamIdsAsync(targetIds),
                ElectionAudienceScope.SelectedEmployees => await _employeeRepository.FilterActiveIdsAsync(targetIds),
                _ => new List<long>()
            };

            return eligibleIds.ToHashSet();
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
