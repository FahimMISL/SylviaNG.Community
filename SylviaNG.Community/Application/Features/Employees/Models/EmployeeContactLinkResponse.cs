using SylviaNG.Community.Domain.Enums;

namespace SylviaNG.Community.Application.Features.Employees.Models
{
    /// <summary>
    /// A contact link visible to the current viewer. Links the viewer isn't allowed to see
    /// (private, viewer isn't owner/HR) are omitted entirely from the response array rather
    /// than included with a null Url - see EmployeeMapper.ToResponse.
    /// </summary>
    public class EmployeeContactLinkResponse
    {
        public long Id { get; set; }
        public string Platform { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public ContactVisibilityEnum Visibility { get; set; }
    }
}
