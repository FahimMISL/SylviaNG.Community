using MediatR;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;

namespace SylviaNG.Community.Application.Features.RecognitionComments.Commands.RecognitionCommentAdd
{
    public class RecognitionCommentAddCommand : IRequest<long>
    {
        public long RecognitionId { get; set; }
        public RecognitionCommentAddRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long CallerEmployeeId { get; set; }

        public RecognitionCommentAddCommand(long recognitionId, RecognitionCommentAddRequest request, long callerEmployeeId)
        {
            RecognitionId = recognitionId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
