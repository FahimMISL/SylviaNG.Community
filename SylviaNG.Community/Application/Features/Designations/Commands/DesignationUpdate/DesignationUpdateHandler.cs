using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationUpdate
{
    public class DesignationUpdateHandler : IRequestHandler<DesignationUpdateCommand>
    {
        private readonly IDesignationService _designationService;

        public DesignationUpdateHandler(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        public async Task Handle(DesignationUpdateCommand command, CancellationToken cancellationToken)
        {
            await _designationService.UpdateAsync(command.DesignationId, command.Request);
        }
    }
}
