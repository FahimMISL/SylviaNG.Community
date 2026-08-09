using MediatR;
using SylviaNG.Community.Application.Features.Recognitions.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Recognitions.Queries.RecognitionGetAllPaged
{
    public class RecognitionGetAllPagedHandler : IRequestHandler<RecognitionGetAllPagedQuery, PagedResult<RecognitionResponse>>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionGetAllPagedHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<PagedResult<RecognitionResponse>> Handle(RecognitionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _recognitionService.GetPaginatedAsync(query.Request, query.SenderId, query.RecipientId);
        }
    }
}
