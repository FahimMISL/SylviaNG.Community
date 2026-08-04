using MediatR;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd
{
    public class RecognitionReactionAddCommand : IRequest<long>
    {
        public long RecognitionId { get; set; }
        public RecognitionReactionAddRequest Request { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long CallerEmployeeId { get; set; }

        public RecognitionReactionAddCommand(long recognitionId, RecognitionReactionAddRequest request, long callerEmployeeId)
        {
            RecognitionId = recognitionId;
            Request = request;
            CallerEmployeeId = callerEmployeeId;
        }
    }
}
