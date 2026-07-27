using MediatR;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Queries.RecurringTaskGetById
{
    public class RecurringTaskGetByIdQuery : IRequest<RecurringTaskResponse>
    {
        public long RecurringTaskId { get; set; }

        public RecurringTaskGetByIdQuery(long recurringTaskId)
        {
            RecurringTaskId = recurringTaskId;
        }
    }
}
