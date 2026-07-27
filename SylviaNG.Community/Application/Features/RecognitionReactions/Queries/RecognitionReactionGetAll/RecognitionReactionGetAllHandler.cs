using MediatR;
using SylviaNG.Community.Application.Features.RecognitionReactions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Queries.RecognitionReactionGetAll
{
    public class RecognitionReactionGetAllHandler : IRequestHandler<RecognitionReactionGetAllQuery, List<RecognitionReactionResponse>>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionReactionGetAllHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<List<RecognitionReactionResponse>> Handle(RecognitionReactionGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _recognitionService.GetReactionsAsync(query.RecognitionId);
        }
    }
}
