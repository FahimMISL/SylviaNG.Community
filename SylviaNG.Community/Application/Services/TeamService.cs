using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Teams.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamMemberRepository _teamMemberRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TeamService(
            ITeamRepository teamRepository,
            ITeamMemberRepository teamMemberRepository,
            IUnitOfWork unitOfWork)
        {
            _teamRepository = teamRepository;
            _teamMemberRepository = teamMemberRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(TeamCreateRequest request)
        {
            var exists = await _teamRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Team", "Name", request.Name);

            var entity = request.ToEntity();
            await _teamRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.TeamId;
        }

        public async Task UpdateAsync(long teamId, TeamUpdateRequest request)
        {
            var entity = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            entity.ApplyUpdate(request);
            _teamRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long teamId)
        {
            var entity = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            _teamRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
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

        public async Task<long> AddMemberAsync(long teamId, TeamMemberAddRequest request)
        {
            _ = await _teamRepository.GetByIdAsync(teamId)
                ?? throw new NotFoundException("Team", teamId);

            var alreadyMember = await _teamMemberRepository.ExistsAsync(teamId, request.EmployeeId);
            if (alreadyMember)
                throw new DuplicateException("TeamMember", "EmployeeId", request.EmployeeId.ToString());

            var entity = request.ToEntity(teamId);
            await _teamMemberRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.TeamMemberId;
        }

        public async Task RemoveMemberAsync(long teamId, long employeeId)
        {
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
