using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;
using TeamEntity = SylviaNG.Community.Domain.Entities.Team;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly INotificationService _notificationService;
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(
            ITeamRepository teamRepository,
            ITeamMemberRepository teamMemberRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
        {
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _notificationService = notificationService;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TeamCreateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            if (!isHrOrAdmin)
            {
                var callerSupervisesATeam = await _teamRepository.ExistsBySupervisorIdAsync(callerEmployeeId);
                if (!callerSupervisesATeam)
                    throw new ForbiddenException("Only HR/Admin, or an employee who already supervises a team, can create a team.");
            }

            var exists = await _teamRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Team", "Name", request.Name);

            var entity = request.ToEntity();
            await _teamRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // US-7.1: the designated supervisor is notified. Members are added via separate
            // AddMemberAsync calls (no member list on create), each notified there.
            if (entity.SupervisorId.HasValue)
            {
                await _notificationService.CreateAsync(new NotificationCreateRequest
                {
                    EmployeeId = entity.SupervisorId.Value,
                    Title = $"You've been made Supervisor of team \"{entity.Name}\"",
                    Category = "Team",
                    RelatedEntityType = "Team",
                    RelatedEntityId = entity.TeamId
                });
            }

            return entity.TeamId;
        }

        public async Task UpdateAsync(long teamId, TeamUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            EnsureSupervisorOrHrAdmin(entity, callerEmployeeId, isHrOrAdmin);

            entity.ApplyUpdate(request);
            _teamRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long teamId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            EnsureSupervisorOrHrAdmin(entity, callerEmployeeId, isHrOrAdmin);

            _teamRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        /// <summary>US-7.2: only the team's own Supervisor, or HR/Admin, may edit/delete it or its roster.</summary>
        private static void EnsureSupervisorOrHrAdmin(TeamEntity entity, long callerEmployeeId, bool isHrOrAdmin)
        {
            if (isHrOrAdmin)
                return;

            if (entity.SupervisorId != callerEmployeeId)
                throw new ForbiddenException("Only this team's Supervisor, or HR/Admin, can do this.");
        }

        public async Task<TeamResponse> GetByIdAsync(long teamId)
        {
            var entity = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<TeamResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _teamRepository.GetPaginatedAsync(request);

            return new PagedResult<TeamResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AddMemberAsync(long teamId, TeamMemberAddRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var team = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            EnsureSupervisorOrHrAdmin(team, callerEmployeeId, isHrOrAdmin);

            var alreadyMember = await _teamMemberRepository.ExistsAsync(teamId, request.EmployeeId);
            if (alreadyMember)
                throw new DuplicateException("TeamMember", "EmployeeId", request.EmployeeId.ToString());

            var entity = request.ToEntity(teamId);
            await _teamMemberRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            // US-7.1/7.2: "every participant is notified" / "newly added people are notified of their role".
            await _notificationService.CreateAsync(new NotificationCreateRequest
            {
                EmployeeId = request.EmployeeId,
                Title = $"You've been added to team \"{team.Name}\"",
                Category = "Team",
                RelatedEntityType = "Team",
                RelatedEntityId = teamId
            });

            return entity.TeamMemberId;
        }

        public async Task RemoveMemberAsync(long teamId, long employeeId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var team = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            EnsureSupervisorOrHrAdmin(team, callerEmployeeId, isHrOrAdmin);

            var entity = await _teamMemberRepository.GetAsync(teamId, employeeId)
                ?? throw new NotFoundException("TeamMember", employeeId);

            entity.IsActive = false;
            _teamMemberRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<TeamMemberResponse>> GetMembersAsync(long teamId)
        {
            var members = await _teamMemberRepository.GetByTeamIdAsync(teamId);
            return members.Select(m => m.ToResponse()).ToList();
        }
    }
}
