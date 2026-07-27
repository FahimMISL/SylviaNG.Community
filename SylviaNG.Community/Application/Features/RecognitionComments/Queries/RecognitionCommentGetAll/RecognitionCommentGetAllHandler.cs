using MediatR;
using SylviaNG.Community.Application.Features.RecognitionComments.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecognitionComments.Queries.RecognitionCommentGetAll
{
    public class RecognitionCommentGetAllHandler : IRequestHandler<RecognitionCommentGetAllQuery, List<RecognitionCommentResponse>>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionCommentGetAllHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<List<RecognitionCommentResponse>> Handle(RecognitionCommentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _recognitionService.GetCommentsAsync(query.RecognitionId);
        }
    }
}
