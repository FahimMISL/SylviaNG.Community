using MediatR;
using SylviaNG.Community.Application.Features.TaskTags.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.TaskTags.Queries.TaskTagGetAllPaged
{
    public class TaskTagGetAllPagedHandler : IRequestHandler<TaskTagGetAllPagedQuery, PagedResult<TaskTagResponse>>
    {
        private readonly ITaskTagService _taskTagService;

        public TaskTagGetAllPagedHandler(ITaskTagService taskTagService)
        {
            _taskTagService = taskTagService;
        }

        public async Task<PagedResult<TaskTagResponse>> Handle(TaskTagGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _taskTagService.GetPaginatedAsync(query.Request);
        }
    }
}
