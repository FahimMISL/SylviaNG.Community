namespace SylviaNG.Community.Application.Features.TaskTags.Models
{
    public class TaskTagResponse
    {
        public long TagId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
