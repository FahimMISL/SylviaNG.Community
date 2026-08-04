using MediatR;

namespace SylviaNG.Community.Application.Features.RecognitionReactions.Commands.RecognitionReactionRemove
{
    public class RecognitionReactionRemoveCommand : IRequest
    {
        public long RecognitionId { get; set; }
        public long EmployeeId { get; set; }

        /// <summary>Populated by the controller from ICurrentUserService - never from client input.</summary>
        public long CallerEmployeeId { get; set; }
        public bool IsHrOrAdmin { get; set; }

        public RecognitionReactionRemoveCommand(long recognitionId, long employeeId, long callerEmployeeId, bool isHrOrAdmin)
        {
            RecognitionId = recognitionId;
            EmployeeId = employeeId;
            CallerEmployeeId = callerEmployeeId;
            IsHrOrAdmin = isHrOrAdmin;
        }
    }
}
