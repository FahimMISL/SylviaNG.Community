using MediatR;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;

namespace SylviaNG.Community.Application.Features.RecognitionComments.Commands.RecognitionCommentAdd
{
    public class RecognitionCommentAddCommand : IRequest<long>
    {
        public long RecognitionId { get; set; }
        public RecognitionCommentAddRequest Request { get; set; }

        public RecognitionCommentAddCommand(long recognitionId, RecognitionCommentAddRequest request)
        {
            RecognitionId = recognitionId;
            Request = request;
        }
    }
}
