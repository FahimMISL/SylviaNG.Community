using MediatR;
using SylviaNG.Community.Application.Features.Designations.Models;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetById
{
    public class DesignationGetByIdHandler : IRequestHandler<DesignationGetByIdQuery, DesignationResponse>
    {
        private readonly IDesignationService _designationService;

        public DesignationGetByIdHandler(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        public async Task<DesignationResponse> Handle(DesignationGetByIdQuery query, CancellationToken cancellationToken)
        {
            return await _designationService.GetByIdAsync(query.DesignationId);
        }
    }
}
