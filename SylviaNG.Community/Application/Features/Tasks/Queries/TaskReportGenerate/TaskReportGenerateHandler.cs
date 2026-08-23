using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskReportGenerate
{
    public class TaskReportGenerateHandler : IRequestHandler<TaskReportGenerateQuery, TaskReportResult>
    {
        private readonly ITaskService _taskService;

        public TaskReportGenerateHandler(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public async Task<TaskReportResult> Handle(TaskReportGenerateQuery query, CancellationToken cancellationToken)
        {
            return await _taskService.GenerateReportAsync(query.TaskId);
        }
    }
}
