using MediatR;
using SylviaNG.Community.Application.Features.Tasks.Models;

namespace SylviaNG.Community.Application.Features.Tasks.Queries.TaskHistoryGetAll
{
    public class TaskHistoryGetAllQuery : IRequest<List<TaskHistoryResponse>>
    {
        public long TaskId { get; set; }

        public TaskHistoryGetAllQuery(long taskId)
        {
            TaskId = taskId;
        }
    }
}
