using MediatR;
using SylviaNG.Community.Application.Features.Mentions.Models;

namespace SylviaNG.Community.Application.Features.Mentions.Queries.MentionGetByEntity
{
    public class MentionGetByEntityQuery : IRequest<List<MentionResponse>>
    {
        public string EntityType { get; set; }
        public long EntityId { get; set; }

        public MentionGetByEntityQuery(string entityType, long entityId)
        {
            EntityType = entityType;
            EntityId = entityId;
        }
    }
}
