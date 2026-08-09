using SylviaNG.Community.Application.Features.ContentReports.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class ContentReportMapper
    {
        public static ContentReport ToEntity(this ContentReportCreateRequest request)
        {
            return new ContentReport
            {
                ReportedBy = request.ReportedBy,
                PostId = request.PostId,
                Reason = request.Reason,
                Status = "Pending"
            };
        }

        public static ContentReportResponse ToResponse(this ContentReport entity)
        {
            return new ContentReportResponse
            {
                ReportId = entity.ReportId,
                ReportedBy = entity.ReportedBy,
                PostId = entity.PostId,
                Reason = entity.Reason,
                Status = entity.Status,
                ReviewedBy = entity.ReviewedBy,
                ReviewedAt = entity.ReviewedAt,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
