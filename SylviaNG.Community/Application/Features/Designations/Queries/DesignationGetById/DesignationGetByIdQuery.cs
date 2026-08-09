using MediatR;
using SylviaNG.Community.Application.Features.Designations.Models;

namespace SylviaNG.Community.Application.Features.Designations.Queries.DesignationGetById
{
    public class DesignationGetByIdQuery : IRequest<DesignationResponse>
    {
        public long DesignationId { get; set; }

        public DesignationGetByIdQuery(long designationId)
        {
            DesignationId = designationId;
        }
    }
}
