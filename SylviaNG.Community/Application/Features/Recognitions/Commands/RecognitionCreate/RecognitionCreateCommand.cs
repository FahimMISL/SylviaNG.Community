using MediatR;
using SylviaNG.Community.Application.Features.Recognitions.Models;

namespace SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate
{
    public class RecognitionCreateCommand : IRequest<long>
    {
        public RecognitionCreateRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public RecognitionCreateCommand(RecognitionCreateRequest request, long callerEmployeeId, bool isHrOrAdmin)
        {
            Request = request;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
