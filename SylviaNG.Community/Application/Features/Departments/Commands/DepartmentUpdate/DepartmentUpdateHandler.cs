using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentUpdate
{
    public class DepartmentUpdateHandler : IRequestHandler<DepartmentUpdateCommand>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentUpdateHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task Handle(DepartmentUpdateCommand command, CancellationToken cancellationToken)
        {
            await _departmentService.UpdateAsync(command.DepartmentId, command.Request);
        }
    }
}
