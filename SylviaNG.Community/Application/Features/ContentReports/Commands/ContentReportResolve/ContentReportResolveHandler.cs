using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportResolve
{
    public class ContentReportResolveHandler : IRequestHandler<ContentReportResolveCommand>
    {
        private readonly IContentReportService _contentReportService;

        public ContentReportResolveHandler(IContentReportService contentReportService)
        {
            _contentReportService = contentReportService;
        }

        public async Task Handle(ContentReportResolveCommand command, CancellationToken cancellationToken)
        {
            await _contentReportService.ResolveAsync(command.ReportId, command.Request);
        }
    }
}
