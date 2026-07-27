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
        private readonly IUnitOfWork _unitOfWork;

        public PostService(IPostRepository postRepository, IUnitOfWork unitOfWork)
        {
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(PostCreateRequest request)
        {
            var entity = request.ToEntity();
            await _postRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.PostId;
        }

        public async Task UpdateAsync(long postId, PostUpdateRequest request)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            if (entity.IsLocked)
                throw new ForbiddenException("This post is locked and cannot be edited.");

            entity.ApplyUpdate(request);
            _postRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteAsync(long postId)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            _postRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<PostResponse> GetByIdAsync(long postId)
        {
            var entity = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<PostResponse>> GetFeedPaginatedAsync(PagedRequest request)
        {
            var pagedResult = await _postRepository.GetFeedPaginatedAsync(request);

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
