using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class PostReactionService : IPostReactionService
    {
        private readonly IPostReactionRepository _reactionRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public PostReactionService(
            IPostReactionRepository reactionRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService)
        {
            _reactionRepository = reactionRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        /// <summary>
        /// Adds a reaction; if the employee already reacted with the same type it is
        /// removed (toggle-off, returns null); if a different type, it is switched.
        /// </summary>
        public async Task<PostReactionResponse?> AddOrToggleAsync(long postId, PostReactionAddRequest request)
        {
            var post = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            var existing = await _reactionRepository.GetAsync(postId, request.EmployeeId);

            if (existing == null)
            {
                var entity = request.ToEntity(postId);
                await _reactionRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                if (post.EmployeeId != request.EmployeeId)
                {
                    await _notificationService.CreateAsync(new NotificationCreateRequest
                    {
                        EmployeeId = post.EmployeeId,
                        Title = "Someone reacted to your post",
                        Category = "PostReaction",
                        RelatedEntityType = "Post",
                        RelatedEntityId = postId
                    });
                }

                return entity.ToResponse();
            }

            if (existing.ReactionType == request.ReactionType)
            {
                _reactionRepository.Delete(existing);
                await _unitOfWork.SaveChangesAsync();
                return null;
            }

            existing.ReactionType = request.ReactionType;
            _reactionRepository.Update(existing);
            await _unitOfWork.SaveChangesAsync();
            return existing.ToResponse();
        }

        public async Task<List<PostReactionResponse>> GetByPostIdAsync(long postId)
        {
            var entities = await _reactionRepository.GetByPostIdAsync(postId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task RemoveAsync(long postId, long employeeId)
        {
            var entity = await _reactionRepository.GetAsync(postId, employeeId)
                ?? throw new NotFoundException("PostReaction", employeeId);

            _reactionRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
