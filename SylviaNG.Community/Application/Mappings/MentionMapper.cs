using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class MentionMapper
    {
        public static Mention ToEntity(this MentionCreateRequest request)
        {
            return new Mention
            {
                MentionedEmployeeId = request.MentionedEmployeeId,
                MentionedBy = request.MentionedBy,
                EntityType = request.EntityType,
                EntityId = request.EntityId
            };
        }

        public static MentionResponse ToResponse(this Mention entity)
        {
            return new MentionResponse
            {
                MentionId = entity.MentionId,
                MentionedEmployeeId = entity.MentionedEmployeeId,
                MentionedBy = entity.MentionedBy,
                EntityType = entity.EntityType,
                EntityId = entity.EntityId,
                CreatedAt = entity.CreatedAt
            };
        }
    }
}
