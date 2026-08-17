using MediatR;
using SylviaNG.Community.Application.Features.Designations.Models;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationUpdate
{
    public class DesignationUpdateCommand : IRequest
    {
        public long DesignationId { get; set; }
        public DesignationUpdateRequest Request { get; set; }

        public DesignationUpdateCommand(long designationId, DesignationUpdateRequest request)
        {
            DesignationId = designationId;
            Request = request;
        }
    }
}
