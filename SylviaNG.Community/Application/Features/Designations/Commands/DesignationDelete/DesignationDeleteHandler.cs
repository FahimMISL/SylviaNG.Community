using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationDelete
{
    public class DesignationDeleteHandler : IRequestHandler<DesignationDeleteCommand>
    {
        private readonly IDesignationService _designationService;

        public DesignationDeleteHandler(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        public async Task Handle(DesignationDeleteCommand command, CancellationToken cancellationToken)
        {
            await _designationService.DeleteAsync(command.DesignationId);
        }
    }
}
