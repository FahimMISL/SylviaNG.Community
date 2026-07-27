using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportCreate
{
    public class ContentReportCreateHandler : IRequestHandler<ContentReportCreateCommand, long>
    {
        private readonly IContentReportService _contentReportService;

        public ContentReportCreateHandler(IContentReportService contentReportService)
        {
            _contentReportService = contentReportService;
        }

        public async Task<long> Handle(ContentReportCreateCommand command, CancellationToken cancellationToken)
        {
            return await _contentReportService.CreateAsync(command.Request);
        }
    }
}
