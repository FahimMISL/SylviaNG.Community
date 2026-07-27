using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionRemove
{
    public class RecognitionReactionRemoveHandler : IRequestHandler<RecognitionReactionRemoveCommand>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionReactionRemoveHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task Handle(RecognitionReactionRemoveCommand command, CancellationToken cancellationToken)
        {
            await _recognitionService.RemoveReactionAsync(command.RecognitionId, command.EmployeeId);
        }
    }
}
