using MediatR;
using SylviaNG.Community.Application.Features.Posts.Models;

namespace SylviaNG.Community.Application.Features.Posts.Commands.PostCreate
{
    public class PostCreateCommand : IRequest<long>
    {
        public PostCreateRequest Request { get; set; }

        public PostCreateCommand(PostCreateRequest request)
        {
            Request = request;
        }
    }
}
