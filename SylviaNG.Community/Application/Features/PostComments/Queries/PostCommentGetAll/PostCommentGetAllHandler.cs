using MediatR;
using SylviaNG.Community.Application.Features.PostComments.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostComments.Queries.PostCommentGetAll
{
    public class PostCommentGetAllHandler : IRequestHandler<PostCommentGetAllQuery, List<PostCommentResponse>>
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentGetAllHandler(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        public async Task<List<PostCommentResponse>> Handle(PostCommentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _postCommentService.GetByPostIdAsync(query.PostId);
        }
    }
}
