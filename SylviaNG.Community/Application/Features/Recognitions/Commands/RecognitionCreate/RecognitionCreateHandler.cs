using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Recognitions.Commands.RecognitionCreate
{
    public class RecognitionCreateHandler : IRequestHandler<RecognitionCreateCommand, long>
    {
        private readonly IRecognitionService _recognitionService;

        public RecognitionCreateHandler(IRecognitionService recognitionService)
        {
            _recognitionService = recognitionService;
        }

        public async Task<long> Handle(RecognitionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _recognitionService.CreateAsync(command.Request);
        }
    }
}
