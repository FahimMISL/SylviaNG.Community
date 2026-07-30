using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.PostComments.Commands.PostCommentUpdate
{
    public class PostCommentUpdateHandler : IRequestHandler<PostCommentUpdateCommand>
    {
        private readonly IPostCommentService _postCommentService;

        public PostCommentUpdateHandler(IPostCommentService postCommentService)
        {
            _postCommentService = postCommentService;
        }

        public async Task Handle(PostCommentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _postCommentService.UpdateAsync(command.PostId, command.CommentId, command.Request, command.CallerEmployeeId, command.IsHrOrAdmin);
        }
    }
}
