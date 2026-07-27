using MediatR;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionRemove
{
    public class RecognitionReactionRemoveCommand : IRequest
    {
        public long RecognitionId { get; set; }
        public long EmployeeId { get; set; }

        public RecognitionReactionRemoveCommand(long recognitionId, long employeeId)
        {
            RecognitionId = recognitionId;
            EmployeeId = employeeId;
        }
    }
}
