using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskAttachmentGetAll
{
    public class TaskAttachmentGetAllHandler : IRequestHandler<TaskAttachmentGetAllQuery, List<TaskAttachmentResponse>>
    {
        private readonly ITaskService _taskService;

        public TaskAttachmentGetAllHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<List<TaskAttachmentResponse>> Handle(TaskAttachmentGetAllQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GetAttachmentsAsync(query.TaskId);
        }
    }
}
