using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.CommentReactions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class CommentReactionService : ICommentReactionService
    {
        private readonly ICommentReactionRepository _reactionRepository;
        private readonly IPostCommentRepository _commentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CommentReactionService(
            ICommentReactionRepository reactionRepository,
            IPostCommentRepository commentRepository,
            IUnitOfWork unitOfWork)
        {
            _reactionRepository = reactionRepository;
            _commentRepository = commentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<CommentReactionResponse?> AddOrToggleAsync(long commentId, CommentReactionAddRequest request)
        {
            _ = await _commentRepository.GetByIdAsync(commentId)
                ?? throw new NotFoundException("PostComment", commentId);

            var existing = await _reactionRepository.GetAsync(commentId, request.EmployeeId);

            if (existing == null)
            {
                var entity = request.ToEntity(commentId);
                await _reactionRepository.AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
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

        public async Task<List<CommentReactionResponse>> GetByCommentIdAsync(long commentId)
        {
            var entities = await _reactionRepository.GetByCommentIdAsync(commentId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task RemoveAsync(long commentId, long employeeId)
        {
            var entity = await _reactionRepository.GetAsync(commentId, employeeId)
                ?? throw new NotFoundException("CommentReaction", employeeId);

            _reactionRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
