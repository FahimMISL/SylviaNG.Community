using MediatR;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;

namespace SylviaNG.Community.Application.Features.RecognitionComments.Queries.RecognitionCommentGetAll
{
    public class RecognitionCommentGetAllQuery : IRequest<List<RecognitionCommentResponse>>
    {
        public long RecognitionId { get; set; }

        public RecognitionCommentGetAllQuery(long recognitionId)
        {
            RecognitionId = recognitionId;
        }
    }
}
