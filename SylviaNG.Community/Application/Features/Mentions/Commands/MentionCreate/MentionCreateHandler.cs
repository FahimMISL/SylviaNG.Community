using MediatR;
using SylviaNG.Community.Application.Interfaces.Services;

namespace SylviaNG.Community.Application.Features.Mentions.Commands.MentionCreate
{
    public class MentionCreateHandler : IRequestHandler<MentionCreateCommand, long>
    {
        private readonly IMentionService _mentionService;

        public MentionCreateHandler(IMentionService mentionService)
        {
            _mentionService = mentionService;
        }

        public async Task<long> Handle(MentionCreateCommand command, CancellationToken cancellationToken)
        {
            return await _mentionService.CreateAsync(command.Request);
        }
    }
}
