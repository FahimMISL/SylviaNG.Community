using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskUpdate
{
    public class RecurringTaskUpdateHandler : IRequestHandler<RecurringTaskUpdateCommand>
    {
        private readonly IRecurringTaskService _recurringTaskService;

        public RecurringTaskUpdateHandler(IRecurringTaskService recurringTaskService)
        {
            _recurringTaskService = recurringTaskService;
        }

        public async Task Handle(RecurringTaskUpdateCommand command, CancellationToken cancellationToken)
        {
            await _recurringTaskService.UpdateAsync(command.RecurringTaskId, command.Request);
        }
    }
}
