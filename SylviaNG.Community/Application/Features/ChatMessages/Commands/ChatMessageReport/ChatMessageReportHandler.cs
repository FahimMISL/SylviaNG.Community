using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatMessages.Commands.ChatMessageReport
{
    public class ChatMessageReportHandler : IRequestHandler<ChatMessageReportCommand, Unit>
    {
        private readonly IChatMessageService _chatMessageService;

        public ChatMessageReportHandler(IChatMessageService chatMessageService)
        {
            _chatMessageService = chatMessageService;
        }

        public async Task<Unit> Handle(ChatMessageReportCommand command, CancellationToken cancellationToken)
        {
            await _chatMessageService.ReportAsync(command.ChatMessageId, command.Request.Reason, command.CallerEmployeeId);
            return Unit.Value;
        }
    }
}
