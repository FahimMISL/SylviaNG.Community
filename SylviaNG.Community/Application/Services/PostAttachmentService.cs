using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.PostAttachments.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;

namespace SylviaNG.Community.Application.Services
{
    public class PostAttachmentService : IPostAttachmentService
    {
        private readonly IPostAttachmentRepository _attachmentRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PostAttachmentService(
            IPostAttachmentRepository attachmentRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork)
        {
            _attachmentRepository = attachmentRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> AddAsync(long postId, PostAttachmentAddRequest request)
        {
            _ = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            var entity = request.ToEntity(postId);
            await _attachmentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.AttachmentId;
        }

        public async Task<List<PostAttachmentResponse>> GetByPostIdAsync(long postId)
        {
            var entities = await _attachmentRepository.GetByPostIdAsync(postId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task RemoveAsync(long postId, long attachmentId)
        {
            var entity = await _attachmentRepository.GetByIdAsync(attachmentId);
            if (entity == null || entity.PostId != postId)
                throw new NotFoundException("PostAttachment", attachmentId);

            _attachmentRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
