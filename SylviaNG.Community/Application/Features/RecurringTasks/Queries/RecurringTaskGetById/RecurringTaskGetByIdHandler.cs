using MediatR;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Queries.RecurringTaskGetById
{
    public class RecurringTaskGetByIdHandler : IRequestHandler<RecurringTaskGetByIdQuery, RecurringTaskResponse>
    {
        private readonly IRecurringTaskService _recurringTaskService;

        public RecurringTaskGetByIdHandler(IRecurringTaskService recurringTaskService)
        {
            _recurringTaskService = recurringTaskService;
        }

        public async Task<RecurringTaskResponse> Handle(RecurringTaskGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _recurringTaskService.GetByIdAsync(query.RecurringTaskId);
        }
    }
}
