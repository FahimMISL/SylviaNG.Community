using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.MessageGetAll
{
    public class MessageGetAllQuery : IRequest<List<MessageResponse>>
    {
        public long ConversationId { get; set; }

        public MessageGetAllQuery(long conversationId)
        {
            ConversationId = conversationId;
        }
    }
}
