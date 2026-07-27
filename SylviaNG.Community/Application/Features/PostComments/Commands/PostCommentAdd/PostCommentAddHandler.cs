using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentAdd
{
    public class PostCommentAddHandler : IRequestHandler<PostCommentAddCommand, long>
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentAddHandler(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        public async Task<long> Handle(PostCommentAddCommand command, CancellationToken cancellationToken)
        {
            return await _postCommentService.AddAsync(command.PostId, command.Request);
        }
    }
}
