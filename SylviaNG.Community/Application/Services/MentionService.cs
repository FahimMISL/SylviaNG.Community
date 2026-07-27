using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class MentionService : IMentionService
    {
        private readonly IMentionRepository _mentionRepository;
        private readonly IUnitOfWork _unitOfWork;

        public MentionService(IMentionRepository mentionRepository, IUnitOfWork unitOfWork)
        {
            _mentionRepository = mentionRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(MentionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _mentionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.MentionId;
        }

        public async Task<PagedResult<MentionResponse>> GetPaginatedForEmployeeAsync(long mentionedEmployeeId, PagedRequest request)
        {
            var pagedResult = await _mentionRepository.GetPaginatedForEmployeeAsync(mentionedEmployeeId, request);

            return new PagedResult<MentionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }
    }
}
