using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class PostCommentService : IPostCommentService
    {
        private readonly IPostCommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostCommentService(
            IPostCommentRepository commentRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> AddAsync(long postId, PostCommentAddRequest request)
        {
            var post = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            if (post.IsLocked)
                throw new ForbiddenException("This post is locked and does not accept new comments.");

            if (request.ParentCommentId.HasValue)
            {
                var parent = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if (parent == null || parent.PostId != postId)
                    throw new NotFoundException("PostComment", request.ParentCommentId.Value);
            }

            var entity = request.ToEntity(postId);
            await _commentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CommentId;
        }

        public async Task<List<PostCommentResponse>> GetByPostIdAsync(long postId)
        {
            var entities = await _commentRepository.GetByPostIdAsync(postId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task DeleteAsync(long postId, long commentId)
        {
            var entity = await _commentRepository.GetByIdAsync(commentId);
            if (entity == null || entity.PostId != postId)
                throw new NotFoundException("PostComment", commentId);

            _commentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
