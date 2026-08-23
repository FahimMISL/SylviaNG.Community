using MediatR;
using SylviaNG.Community.Application.Features.Departments.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Departments.Queries.DepartmentGetById
{
    public class DepartmentGetByIdHandler : IRequestHandler<DepartmentGetByIdQuery, DepartmentResponse>
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentGetByIdHandler(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<DepartmentResponse> Handle(DepartmentGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _departmentService.GetByIdAsync(query.DepartmentId);
        }
    }
}
