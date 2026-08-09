using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentDelete
{
    public class PostCommentDeleteHandler : IRequestHandler<PostCommentDeleteCommand>
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentDeleteHandler(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        public async Task Handle(PostCommentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _postCommentService.DeleteAsync(command.PostId, command.CommentId, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
