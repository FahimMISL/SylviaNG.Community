using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.MessageSend
{
    public class MessageSendCommand : IRequest<long>
    {
        public long ConversationId { get; set; }
        public long SenderId { get; set; }
        public MessageSendRequest Request { get; set; }

        public MessageSendCommand(long conversationId, long senderId, MessageSendRequest request)
        {
            ConversationId = conversationId;
            SenderId = senderId;
            Request = request;
        }
    }
}
