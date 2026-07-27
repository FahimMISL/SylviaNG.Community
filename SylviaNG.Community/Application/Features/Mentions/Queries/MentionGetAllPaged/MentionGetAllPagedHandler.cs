using MediatR;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetAllPaged
{
    public class MentionGetAllPagedHandler : IRequestHandler<MentionGetAllPagedQuery, PagedResult<MentionResponse>>
    {
        private readonly IMentionService _mentionService;

        public MentionGetAllPagedHandler(IMentionService mentionService)
        {
            _mentionService = mentionService;
        }

        public async Task<PagedResult<MentionResponse>> Handle(MentionGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _mentionService.GetPaginatedForEmployeeAsync(query.MentionedEmployeeId, query.Request);
        }
    }
}
