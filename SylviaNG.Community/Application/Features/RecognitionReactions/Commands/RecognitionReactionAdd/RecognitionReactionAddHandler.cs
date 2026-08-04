using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionAdd
{
    public class RecognitionReactionAddHandler : IRequestHandler<RecognitionReactionAddCommand, long>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionReactionAddHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<long> Handle(RecognitionReactionAddCommand command, CancellationToken cancellationToken)
        {
            return await _recognitionService.AddReactionAsync(command.RecognitionId, command.Request, command.CallerEmployeeId);
        }
    }
}
