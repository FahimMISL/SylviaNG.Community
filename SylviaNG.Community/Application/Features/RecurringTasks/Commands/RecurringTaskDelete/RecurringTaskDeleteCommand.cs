using MediatR;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskDelete
{
    public class RecurringTaskDeleteCommand : IRequest
    {
        public long RecurringTaskId { get; set; }

        public RecurringTaskDeleteCommand(long recurringTaskId)
        {
            RecurringTaskId = recurringTaskId;
        }
    }
}
