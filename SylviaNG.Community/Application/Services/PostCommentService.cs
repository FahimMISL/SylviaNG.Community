using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Notifications.Models;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class PostCommentService : IPostCommentService
    {
        private readonly IPostCommentRepository _commentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IMentionService _mentionService;

        public PostCommentService(
            IPostCommentRepository commentRepository,
            IPostRepository postRepository,
            IEmployeeRepository employeeRepository,
            IUnitOfWork unitOfWork,
            INotificationService notificationService,
            IMentionService mentionService)
        {
            _commentRepository = commentRepository;
            _postRepository = postRepository;
            _employeeRepository = employeeRepository;
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _mentionService = mentionService;
        }

        public async Task<long> AddAsync(long postId, PostCommentAddRequest request)
        {
            var post = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            if (post.IsLocked)
                throw new ForbiddenException("This post is locked and does not accept new comments.");

            PostComment? parent = null;
            if (request.ParentCommentId.HasValue)
            {
                parent = await _commentRepository.GetByIdAsync(request.ParentCommentId.Value);
                if (parent == null || parent.PostId != postId)
                    throw new NotFoundException("PostComment", request.ParentCommentId.Value);
            }

            var entity = request.ToEntity(postId);
            await _commentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            if ((post.EmployeeId != request.EmployeeId) || (parent != null && parent.EmployeeId != request.EmployeeId && parent.EmployeeId != post.EmployeeId))
            {
                var commenterName = (await _employeeRepository.GetByIdAsync(request.EmployeeId))?.EmployeeName ?? "Someone";

                if (post.EmployeeId != request.EmployeeId)
                {
                    await _notificationService.CreateAsync(new NotificationCreateRequest
                    {
                        EmployeeId = post.EmployeeId,
                        Title = $"{commenterName} commented on your post",
                        Message = request.Content,
                        Category = "PostComment",
                        RelatedEntityType = "Post",
                        RelatedEntityId = postId
                    });
                }

                if (parent != null && parent.EmployeeId != request.EmployeeId && parent.EmployeeId != post.EmployeeId)
                {
                    await _notificationService.CreateAsync(new NotificationCreateRequest
                    {
                        EmployeeId = parent.EmployeeId,
                        Title = $"{commenterName} replied to your comment",
                        Message = request.Content,
                        Category = "CommentReply",
                        RelatedEntityType = "PostComment",
                        RelatedEntityId = parent.CommentId
                    });
                }
            }

            await _mentionService.CreateMentionsAsync("PostComment", entity.CommentId, request.EmployeeId, request.MentionedEmployeeIds);

            return entity.CommentId;
        }

        public async Task<List<PostCommentResponse>> GetByPostIdAsync(long postId)
        {
            var entities = await _commentRepository.GetByPostIdAsync(postId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task UpdateAsync(long postId, long commentId, PostCommentUpdateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _commentRepository.GetByIdAsync(commentId);
            if (entity == null || entity.PostId != postId)
                throw new NotFoundException("PostComment", commentId);

            if (!isHrOrAdmin && entity.EmployeeId != callerEmployeeId)
                throw new ForbiddenException("You can only edit your own comment.");

            var post = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            if (post.IsLocked)
                throw new ForbiddenException("This post is locked and its comments cannot be edited.");

            entity.ApplyUpdate(request);
            _commentRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            await _mentionService.CreateMentionsAsync("PostComment", entity.CommentId, entity.EmployeeId, request.MentionedEmployeeIds);
        }

        public async Task DeleteAsync(long postId, long commentId, long callerEmployeeId, bool isHrOrAdmin)
        {
            var entity = await _commentRepository.GetByIdAsync(commentId);
            if (entity == null || entity.PostId != postId)
                throw new NotFoundException("PostComment", commentId);

            if (!isHrOrAdmin && entity.EmployeeId != callerEmployeeId)
                throw new ForbiddenException("You can only delete your own comment.");

            _commentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
