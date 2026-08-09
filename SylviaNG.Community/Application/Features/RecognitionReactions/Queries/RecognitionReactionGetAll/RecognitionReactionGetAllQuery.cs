using MediatR;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Queries.RecognitionReactionGetAll
{
    public class RecognitionReactionGetAllQuery : IRequest<List<RecognitionReactionResponse>>
    {
        public long RecognitionId { get; set; }

        public RecognitionReactionGetAllQuery(long recognitionId)
        {
            RecognitionId = recognitionId;
        }
    }
}
