using MediatR;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetAllPaged
{
    public class RecognitionGetAllPagedQuery : IRequest<PagedResult<RecognitionResponse>>
    {
        public PagedRequest Request { get; set; }
        public long? SenderId { get; set; }
        public long? RecipientId { get; set; }
        public long? ViewerEmployeeId { get; set; }
        public bool ViewerIsHrAdmin { get; set; }

        public RecognitionGetAllPagedQuery(PagedRequest request, long? senderId = null, long? recipientId = null, long? viewerEmployeeId = null, bool viewerIsHrAdmin = false)
        {
            Request = request;
            SenderId = senderId;
            RecipientId = recipientId;
            ViewerEmployeeId = viewerEmployeeId;
            ViewerIsHrAdmin = viewerIsHrAdmin;
        }
    }
}
