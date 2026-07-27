using MediatR;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetById
{
    public class RecognitionGetByIdHandler : IRequestHandler<RecognitionGetByIdQuery, RecognitionResponse>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionGetByIdHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<RecognitionResponse> Handle(RecognitionGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _recognitionService.GetByIdAsync(query.RecognitionId);
        }
    }
}
