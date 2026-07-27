using MediatR;
using SylviaNG.Community.Application.Features.RecurringTasks.Models;

namespace SylviaNG.Community.Application.Features.RecurringTasks.Commands.RecurringTaskCreate
{
    public class RecurringTaskCreateCommand : IRequest<long>
    {
        public RecurringTaskCreateRequest Request { get; set; }

        public RecurringTaskCreateCommand(RecurringTaskCreateRequest request)
        {
            Request = request;
        }
    }
}
