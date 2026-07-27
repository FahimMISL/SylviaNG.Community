using MediatR;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd
{
    public class RecognitionReactionAddCommand : IRequest<long>
    {
        public long RecognitionId { get; set; }
        public RecognitionReactionAddRequest Request { get; set; }

        public RecognitionReactionAddCommand(long recognitionId, RecognitionReactionAddRequest request)
        {
            RecognitionId = recognitionId;
            Request = request;
        }
    }
}
