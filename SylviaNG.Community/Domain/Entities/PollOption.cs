using SylviaNG.Community.SharedKernel.Audit;

namespace SylviaNG.Community.Domain.Entities;

public class PollOption : Audit
{
    public long PollOptionId { get; set; }
    public long PollId { get; set; }
    public string OptionText { get; set; } = string.Empty;
}
