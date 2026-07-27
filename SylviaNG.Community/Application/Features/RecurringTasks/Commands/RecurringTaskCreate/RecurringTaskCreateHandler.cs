using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskCreate
{
    public class RecurringTaskCreateHandler : IRequestHandler<RecurringTaskCreateCommand, long>
    {
        private readonly IRecurringTaskService _recurringTaskService;

        public RecurringTaskCreateHandler(IRecurringTaskService recurringTaskService)
        {
            _recurringTaskService = recurringTaskService;
        }

        public async Task<long> Handle(RecurringTaskCreateCommand command, CancellationToken cancellationToken)
        {
            return await _recurringTaskService.CreateAsync(command.Request);
        }
    }
}
