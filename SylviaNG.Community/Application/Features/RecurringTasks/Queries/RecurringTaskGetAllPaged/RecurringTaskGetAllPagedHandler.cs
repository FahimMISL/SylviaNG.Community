using MediatR;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Queries.RecurringTaskGetAllPaged
{
    public class RecurringTaskGetAllPagedHandler : IRequestHandler<RecurringTaskGetAllPagedQuery, PagedResult<RecurringTaskResponse>>
    {
        private readonly IRecurringTaskService _recurringTaskService;

        public RecurringTaskGetAllPagedHandler(IRecurringTaskService recurringTaskService)
        {
            _recurringTaskService = recurringTaskService;
        }

        public async Task<PagedResult<RecurringTaskResponse>> Handle(RecurringTaskGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _recurringTaskService.GetPaginatedAsync(query.Request);
        }
    }
}
