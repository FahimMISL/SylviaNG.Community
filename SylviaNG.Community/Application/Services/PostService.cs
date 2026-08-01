using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Posts.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class PostService : IPostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMentionService _mentionService;

        public PostService(IPostRepository postRepository, IEmployeeRepository employeeRepository, IUnitOfWork unitOfWork, IMentionService mentionService)
        {
            _postRepository = postRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _mentionService = mentionService;
        }

        public async Task<long> CreateAsync(PostCreateRequest request)
        {
            var entity = request.ToEntity();
            await _postRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            await _mentionService.CreateMentionsAsync("Post", entity.PostId, request.EmployeeId, request.MentionedEmployeeIds);

            return entity.PostId;
        }

        public async Task UpdateAsync(long postId, PostUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            if (!isHrOrAdmin && entity.EmployeeId != callerEmployeeId)
                throw new ForbiddenException("You can only edit your own post.");

            if (entity.IsLocked)
                throw new ForbiddenException("This post is locked and cannot be edited.");

            entity.ApplyUpdate(request);
            _postRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            await _mentionService.CreateMentionsAsync("Post", entity.PostId, entity.EmployeeId, request.MentionedEmployeeIds);
        }

        public async Task DeleteAsync(long postId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            if (!isHrOrAdmin && entity.EmployeeId != callerEmployeeId)
                throw new ForbiddenException("You can only delete your own post.");

            _postRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PostResponse> GetByIdAsync(long postId)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<PostResponse>> GetFeedPaginatedAsync(PostFilterRequest request, long callerEmployeeId)
        {
            var caller = await _employeeRepository.GetByIdAsync(callerEmployeeId);
            var pagedResult = await _postRepository.GetFeedPaginatedAsync(request, caller?.DepartmentId, caller?.SiteId);

            return new PagedResult<PostResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task SetLockedAsync(long postId, bool isLocked)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            entity.IsLocked = isLocked;
            _postRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SetHiddenAsync(long postId, bool isHidden)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            entity.IsHidden = isHidden;
            _postRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
