using MediatR;
using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetByEntity
{
    public class MentionGetByEntityHandler : IRequestHandler<MentionGetByEntityQuery, List<MentionResponse>>
    {
        private readonly IMentionService _mentionService;

        public MentionGetByEntityHandler(IMentionService mentionService)
        {
            _mentionService = mentionService;
        }

        public async Task<List<MentionResponse>> Handle(MentionGetByEntityQuery query, CancellationToken cancellationToken)
        {
            return await _mentionService.GetByEntityAsync(query.EntityType, query.EntityId);
        }
    }
}
