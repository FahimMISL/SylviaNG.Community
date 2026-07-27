using MediatR;
using SylviaNG.Community.Application.Features.Mentions.Models;

namespace SylviaNG.Community.Application.Features.Mentions.Commands.MentionCreate
{
    public class MentionCreateCommand : IRequest<long>
    {
        public MentionCreateRequest Request { get; set; }

        public MentionCreateCommand(MentionCreateRequest request)
        {
            Request = request;
        }
    }
}
