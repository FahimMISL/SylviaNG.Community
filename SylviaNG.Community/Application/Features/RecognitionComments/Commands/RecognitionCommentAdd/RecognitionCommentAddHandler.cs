using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.RecognitionComments.Commands.RecognitionCommentAdd
{
    public class RecognitionCommentAddHandler : IRequestHandler<RecognitionCommentAddCommand, long>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionCommentAddHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<long> Handle(RecognitionCommentAddCommand command, CancellationToken cancellationToken)
        {
            return await _recognitionService.AddCommentAsync(command.RecognitionId, command.Request);
        }
    }
}
