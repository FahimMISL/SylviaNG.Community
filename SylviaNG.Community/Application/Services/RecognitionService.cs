using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Services
{
    public class RecognitionService : IRecognitionService
    {
        private readonly IRecognitionRepository _recognitionRepository;
        private readonly IRecognitionReactionRepository _recognitionReactionRepository;
        private readonly IRecognitionCommentRepository _recognitionCommentRepository;
        private readonly IUnitOfWork _unitOfWork;

        public RecognitionService(
            IRecognitionRepository recognitionRepository,
            IRecognitionReactionRepository recognitionReactionRepository,
            IRecognitionCommentRepository recognitionCommentRepository,
            IUnitOfWork unitOfWork)
        {
            _recognitionRepository = recognitionRepository;
            _recognitionReactionRepository = recognitionReactionRepository;
            _recognitionCommentRepository = recognitionCommentRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(RecognitionCreateRequest request)
        {
            var entity = request.ToEntity();
            await _recognitionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.RecognitionId;
        }

        public async Task<RecognitionResponse> GetByIdAsync(long recognitionId)
        {
            var entity = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            return entity.ToResponse();
        }

        public async Task<PagedResult<RecognitionResponse>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null)
        {
            var pagedResult = await _recognitionRepository.GetPaginatedAsync(request, senderId, recipientId);

            return new PagedResult<RecognitionResponse>
            {
                Data = pagedResult.Data.Select(e => e.ToResponse()).ToList(),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<long> AddReactionAsync(long recognitionId, RecognitionReactionAddRequest request)
        {
            _ = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var existing = await _recognitionReactionRepository.GetAsync(recognitionId, request.EmployeeId);
            if (existing != null)
            {
                existing.ReactionType = request.ReactionType;
                _recognitionReactionRepository.Update(existing);
                await _unitOfWork.SaveChangesAsync();
                return existing.ReactionId;
            }

            var entity = request.ToEntity(recognitionId);
            await _recognitionReactionRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.ReactionId;
        }

        public async Task RemoveReactionAsync(long recognitionId, long employeeId)
        {
            var entity = await _recognitionReactionRepository.GetAsync(recognitionId, employeeId)
                ?? throw new NotFoundException("RecognitionReaction", employeeId);

            _recognitionReactionRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<RecognitionReactionResponse>> GetReactionsAsync(long recognitionId)
        {
            var entities = await _recognitionReactionRepository.GetByRecognitionIdAsync(recognitionId);
            return entities.Select(e => e.ToResponse()).ToList();
        }

        public async Task<long> AddCommentAsync(long recognitionId, RecognitionCommentAddRequest request)
        {
            _ = await _recognitionRepository.GetByIdAsync(recognitionId)
                ?? throw new NotFoundException("Recognition", recognitionId);

            var entity = request.ToEntity(recognitionId);
            await _recognitionCommentRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return entity.CommentId;
        }

        public async Task<List<RecognitionCommentResponse>> GetCommentsAsync(long recognitionId)
        {
            var entities = await _recognitionCommentRepository.GetByRecognitionIdAsync(recognitionId);
            return entities.Select(e => e.ToResponse()).ToList();
        }
    }
}
