using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentDelete
{
    public class DepartmentDeleteHandler : IRequestHandler<DepartmentDeleteCommand>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentDeleteHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task Handle(DepartmentDeleteCommand command, CancellationToken cancellationToken)
        {
            await _departmentService.DeleteAsync(command.DepartmentId);
        }
    }
}
