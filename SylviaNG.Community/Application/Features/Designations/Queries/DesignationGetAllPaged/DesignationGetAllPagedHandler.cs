using MediatR;
using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.Application.Interfaces.Services;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetAllPaged
{
    public class DesignationGetAllPagedHandler : IRequestHandler<DesignationGetAllPagedQuery, PagedResult<DesignationResponse>>
    {
        private readonly IDesignationService _designationService;

        public DesignationGetAllPagedHandler(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        public async Task<PagedResult<DesignationResponse>> Handle(DesignationGetAllPagedQuery query, CancellationToken cancellationToken)
        {
            return await _designationService.GetPaginatedAsync(query.Request);
        }
    }
}
