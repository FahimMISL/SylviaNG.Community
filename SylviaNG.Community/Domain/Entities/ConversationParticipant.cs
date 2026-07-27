using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class ConversationParticipant : Audit
{
    public long ParticipantId { get; set; }
    public long ConversationId { get; set; }
    public long EmployeeId { get; set; }
}
