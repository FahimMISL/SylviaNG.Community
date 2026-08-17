using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Designations.Commands.DesignationCreate
{
    public class DesignationCreateHandler : IRequestHandler<DesignationCreateCommand, long>
    {
        private readonly IDesignationService _designationService;

        public DesignationCreateHandler(IDesignationService designationService)
        {
            _designationService = designationService;
        }

        public async Task<long> Handle(DesignationCreateCommand command, CancellationToken cancellationToken)
        {
            return await _designationService.CreateAsync(command.Request);
        }
    }
}
