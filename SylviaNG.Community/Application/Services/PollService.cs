using SylviaNG.Community.Application.Common.Exceptions;
using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.Application.Mappings;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Application.Services
{
    public class PollService : IPollService
    {
        private readonly IPollRepository _pollRepository;
        private readonly IPollOptionRepository _pollOptionRepository;
        private readonly IPollVoteRepository _pollVoteRepository;
        private readonly IPostRepository _postRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PollService(
            IPollRepository pollRepository,
            IPollOptionRepository pollOptionRepository,
            IPollVoteRepository pollVoteRepository,
            IPostRepository postRepository,
            IUnitOfWork unitOfWork)
        {
            _pollRepository = pollRepository;
            _pollOptionRepository = pollOptionRepository;
            _pollVoteRepository = pollVoteRepository;
            _postRepository = postRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<long> CreateAsync(long postId, PollCreateRequest request)
        {
            var post = await _postRepository.GetByIdAsync(postId)
                ?? throw new NotFoundException("Post", postId);

            var alreadyExists = await _pollRepository.ExistsByPostIdAsync(postId);
            if (alreadyExists)
                throw new DuplicateException("Poll", "PostId", postId.ToString());

            var poll = request.ToEntity(postId);
            await _pollRepository.AddAsync(poll);
            await _unitOfWork.SaveChangesAsync();

            foreach (var optionText in request.Options)
            {
                await _pollOptionRepository.AddAsync(optionText.ToOptionEntity(poll.PollId));
            }

            post.IsPoll = true;
            _postRepository.Update(post);

            await _unitOfWork.SaveChangesAsync();

            return poll.PollId;
        }

        public async Task<PollResponse> GetByPostIdAsync(long postId)
        {
            var poll = await _pollRepository.GetByPostIdAsync(postId)
                ?? throw new NotFoundException("Poll", postId);

            var options = await _pollOptionRepository.GetByPollIdAsync(poll.PollId);
            var optionIds = options.Select(o => o.PollOptionId).ToList();
            var votes = await _pollVoteRepository.GetByOptionsAsync(optionIds);

            var optionResponses = options
                .Select(o => o.ToResponse(votes.Count(v => v.PollOptionId == o.PollOptionId)))
                .ToList();

            return poll.ToResponse(optionResponses);
        }

        public async Task<long> VoteAsync(long postId, PollVoteRequest request)
        {
            var poll = await _pollRepository.GetByPostIdAsync(postId)
                ?? throw new NotFoundException("Poll", postId);

            if (poll.ExpirationDate.HasValue && poll.ExpirationDate.Value < DateTime.UtcNow)
                throw new ForbiddenException("This poll has expired and no longer accepts votes.");

            var options = await _pollOptionRepository.GetByPollIdAsync(poll.PollId);
            var optionIds = options.Select(o => o.PollOptionId).ToList();

            if (!optionIds.Contains(request.PollOptionId))
                throw new NotFoundException("PollOption", request.PollOptionId);

            var existingVote = await _pollVoteRepository.GetByEmployeeAndOptionsAsync(request.EmployeeId, optionIds);

            if (existingVote != null)
            {
                if (!poll.AllowVoteChange)
                    throw new ForbiddenException("This poll does not allow changing your vote.");

                existingVote.PollOptionId = request.PollOptionId;
                existingVote.VotedAt = DateTime.UtcNow;
                _pollVoteRepository.Update(existingVote);
                await _unitOfWork.SaveChangesAsync();

                return existingVote.VoteId;
            }

            var vote = new PollVote
            {
                PollOptionId = request.PollOptionId,
                EmployeeId = request.EmployeeId,
                VotedAt = DateTime.UtcNow
            };

            await _pollVoteRepository.AddAsync(vote);
            await _unitOfWork.SaveChangesAsync();

            return vote.VoteId;
        }
    }
}
