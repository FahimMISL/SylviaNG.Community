using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskDelete
{
    public class RecurringTaskDeleteHandler : IRequestHandler<RecurringTaskDeleteCommand>
    {
        private readonly IRecurringTaskService _recurringTaskService;

        public RecurringTaskDeleteHandler(IRecurringTaskService recurringTaskService)
        {
            _recurringTaskService = recurringTaskService;
        }

        public async Task Handle(RecurringTaskDeleteCommand command, CancellationToken cancellationToken)
        {
            await _recurringTaskService.DeleteAsync(command.RecurringTaskId);
        }
    }
}
