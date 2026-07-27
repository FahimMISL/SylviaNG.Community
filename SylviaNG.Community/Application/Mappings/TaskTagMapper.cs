using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Domain.Entities;

namespace SylviaNG.Community.Application.Mappings
{
    public static class TaskTagMapper
    {
        public static TaskTag ToEntity(this TaskTagCreateRequest request)
        {
            return new TaskTag
            {
                Name = request.Name,
                Description = request.Description
            };
        }

        public static TaskTagResponse ToResponse(this TaskTag entity)
        {
            return new TaskTagResponse
            {
                TagId = entity.TagId,
                Name = entity.Name,
                Description = entity.Description
            };
        }
    }
}
