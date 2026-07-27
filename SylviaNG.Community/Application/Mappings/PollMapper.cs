using SylviaNG.Community.Application.Features.Polls.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class PollMapper
    {
        public static Poll ToEntity(this PollCreateRequest request, long postId)
        {
            return new Poll
            {
                PostId = postId,
                AllowVoteChange = request.AllowVoteChange,
                ExpirationDate = request.ExpirationDate
            };
        }

        public static PollOption ToOptionEntity(this string optionText, long pollId)
        {
            return new PollOption
            {
                PollId = pollId,
                OptionText = optionText
            };
        }

        public static PollOptionResponse ToResponse(this PollOption entity, int voteCount)
        {
            return new PollOptionResponse
            {
                PollOptionId = entity.PollOptionId,
                PollId = entity.PollId,
                OptionText = entity.OptionText,
                VoteCount = voteCount
            };
        }

        public static PollResponse ToResponse(this Poll entity, List<PollOptionResponse> options)
        {
            return new PollResponse
            {
                PollId = entity.PollId,
                PostId = entity.PostId,
                AllowVoteChange = entity.AllowVoteChange,
                ExpirationDate = entity.ExpirationDate,
                Options = options
            };
        }

        public static PollVoteResponse ToResponse(this PollVote entity)
        {
            return new PollVoteResponse
            {
                VoteId = entity.VoteId,
                PollOptionId = entity.PollOptionId,
                EmployeeId = entity.EmployeeId,
                VotedAt = entity.VotedAt
            };
        }
    }
}
