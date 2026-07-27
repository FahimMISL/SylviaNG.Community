using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.EmployeeSkills.Models;
using SylviaNG.Community.Application.Features.Skills.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class SkillService : ISkillService
    {
        private readonly ISkillRepository _skillRepository;
        private readonly IEmployeeSkillRepository _employeeSkillRepository;
        private readonly IUnitOfWork _unitOfWork;

        public SkillService(
            ISkillRepository skillRepository,
            IEmployeeSkillRepository employeeSkillRepository,
            IUnitOfWork unitOfWork)
        {
            _skillRepository = skillRepository;
            _employeeSkillRepository = employeeSkillRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(SkillCreateRequest request)
        {
            var exists = await _skillRepository.ExistsByNameAsync(request.Name);
            if (exists)
                throw new DuplicateException("Skill", "Name", request.Name);

            var entity = request.ToEntity();
            await _skillRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.SkillId;
        }

        public async Task DeleteAsync(long skillId)
        {
            var entity = await _skillRepository.GetByIdAsync(skillId)
                ?? throw new NotFoundException("Skill", skillId);

            _skillRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<SkillResponse> GetByIdAsync(long skillId)
        {
            var entity = await _skillRepository.GetByIdAsync(skillId)
                ?? throw new NotFoundException("Skill", skillId);

            return entity.ToResponse();
        }

        public async Task<List<SkillResponse>> GetAllAsync()
        {
            var entities = await _skillRepository.GetAllAsync();
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<PagedResult<SkillResponse>> GetPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _skillRepository.GetPaginatedAsync(request);

            return new PagedResult<SkillResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AssignToEmployeeAsync(long employeeId, EmployeeSkillAssignRequest request)
        {
            _ = await _skillRepository.GetByIdAsync(request.SkillId)
                ?? throw new NotFoundException("Skill", request.SkillId);

            var alreadyAssigned = await _employeeSkillRepository.ExistsAsync(employeeId, request.SkillId);
            if (alreadyAssigned)
                throw new DuplicateException("EmployeeSkill", "SkillId", request.SkillId.ToString());

            var entity = request.ToEntity(employeeId);
            await _employeeSkillRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.EmployeeSkillId;
        }

        public async Task RemoveFromEmployeeAsync(long employeeId, long skillId)
        {
            var entity = await _employeeSkillRepository.GetAsync(employeeId, skillId)
                ?? throw new NotFoundException("EmployeeSkill", skillId);

            _employeeSkillRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<EmployeeSkillResponse>> GetEmployeeSkillsAsync(long employeeId)
        {
            var entities = await _employeeSkillRepository.GetByEmployeeIdAsync(employeeId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
