using MediatR;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationDelete
{
    public class DesignationDeleteCommand : IRequest
    {
        public long DesignationId { get; set; }

        public DesignationDeleteCommand(long designationId)
        {
            DesignationId = designationId;
        }
    }
}
