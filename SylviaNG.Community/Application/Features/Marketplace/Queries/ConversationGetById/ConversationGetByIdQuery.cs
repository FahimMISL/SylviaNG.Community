using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Queries.ConversationGetById
{
    public class ConversationGetByIdQuery : IRequest<ConversationResponse>
    {
        public long ConversationId { get; set; }

        public ConversationGetByIdQuery(long conversationId)
        {
            ConversationId = conversationId;
        }
    }
}
