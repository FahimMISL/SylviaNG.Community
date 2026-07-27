using MediatR;
using SylviaNG.Community.Application.Features.PostReactions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostReactions.Queries.PostReactionGetAll
{
    public class PostReactionGetAllHandler : IRequestHandler<PostReactionGetAllQuery, List<PostReactionResponse>>
    {
        private readonly IPostReactionService _postReactionService;

        public PostReactionGetAllHandler(IPostReactionService postReactionService)
        {
            _postReactionService = postReactionService;
        }

        public async Task<List<PostReactionResponse>> Handle(PostReactionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _postReactionService.GetByPostIdAsync(query.PostId);
        }
    }
}
