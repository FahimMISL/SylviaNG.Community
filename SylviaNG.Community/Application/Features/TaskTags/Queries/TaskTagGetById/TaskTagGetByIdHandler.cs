using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.TaskTags.Queries.TaskTagGetById
{
    public class TaskTagGetByIdHandler : IRequestHandler<TaskTagGetByIdQuery, TaskTagResponse>
    {
        private readonly ITaskTagService _taskTagService;

        public TaskTagGetByIdHandler(ITaskTagService taskTagService)
        {
            _taskTagService = taskTagService;
        }

        public async Task<TaskTagResponse> Handle(TaskTagGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _taskTagService.GetByIdAsync(query.TagId);
        }
    }
}
