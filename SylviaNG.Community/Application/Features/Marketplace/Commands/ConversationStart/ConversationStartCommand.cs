using MediatR;
using SylviaNG.Community.Application.Features.Marketplace.Models;

namespace SylviaNG.Community.Application.Features.Marketplace.Commands.ConversationStart
{
    public class ConversationStartCommand : IRequest<long>
    {
        public long InitiatorEmployeeId { get; set; }
        public ConversationStartRequest Request { get; set; }

        public ConversationStartCommand(long initiatorEmployeeId, ConversationStartRequest request)
        {
            InitiatorEmployeeId = initiatorEmployeeId;
            Request = request;
        }
    }
}
