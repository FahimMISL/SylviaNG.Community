using MediatR;
using SylviaNG.Community.Application.Features.ContentReports.Models;

namespace SylviaNG.Community.Application.Features.ContentReports.Commands.ContentReportCreate
{
    public class ContentReportCreateCommand : IRequest<long>
    {
        public ContentReportCreateRequest Request { get; set; }

        public ContentReportCreateCommand(ContentReportCreateRequest request)
        {
            Request = request;
        }
    }
}
