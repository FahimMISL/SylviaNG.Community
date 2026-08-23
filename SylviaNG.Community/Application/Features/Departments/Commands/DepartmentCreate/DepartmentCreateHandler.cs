using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Departments.Commands.DepartmentCreate
{
    public class DepartmentCreateHandler : IRequestHandler<DepartmentCreateCommand, long>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentCreateHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<long> Handle(DepartmentCreateCommand command, CancellationToken cancellationToken)
        {
            return await _departmentService.CreateAsync(command.Request);
        }
    }
}
