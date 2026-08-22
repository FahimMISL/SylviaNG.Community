using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskReportGenerate
{
    public class TaskReportGenerateQuery : IRequest<TaskReportResult>
    {
        public long TaskId { get; set; }

        public TaskReportGenerateQuery(long taskId)
        {
            TaskId = taskId;
        }
    }
}
