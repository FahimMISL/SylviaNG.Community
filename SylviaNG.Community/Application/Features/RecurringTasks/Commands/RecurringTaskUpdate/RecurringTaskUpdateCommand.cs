using MediatR;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskUpdate
{
    public class RecurringTaskUpdateCommand : IRequest
    {
        public long RecurringTaskId { get; set; }
        public RecurringTaskUpdateRequest Request { get; set; }

        public RecurringTaskUpdateCommand(long recurringTaskId, RecurringTaskUpdateRequest request)
        {
            RecurringTaskId = recurringTaskId;
            Request = request;
        }
    }
}
