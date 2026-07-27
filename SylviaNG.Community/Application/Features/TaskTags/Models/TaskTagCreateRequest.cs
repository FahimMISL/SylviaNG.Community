namespace SylviaNG.Community.Application.Features.TaskTags.Models
{
    public class TaskTagCreateRequest
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
