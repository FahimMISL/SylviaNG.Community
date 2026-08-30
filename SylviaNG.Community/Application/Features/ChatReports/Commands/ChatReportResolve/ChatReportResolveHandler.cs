using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ChatReports.Commands.ChatReportResolve
{
    public class ChatReportResolveHandler : IRequestHandler<ChatReportResolveCommand>
    {
        private readonly IChatReportService _chatReportService;

        public ChatReportResolveHandler(IChatReportService chatReportService)
        {
            _chatReportService = chatReportService;
        }

        public async Task Handle(ChatReportResolveCommand command, CancellationToken cancellationToken)
        {
            await _chatReportService.ResolveAsync(command.ReportId, command.Request);
        }
    }
}
