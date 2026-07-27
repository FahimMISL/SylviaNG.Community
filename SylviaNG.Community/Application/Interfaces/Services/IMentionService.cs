using SylviaNG.Community.Application.Features.Mentions.Models;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Services
{
    public interface IMentionService
    {
        Task<long> CreateAsync(MentionCreateRequest request);
        Task<PagedResult<MentionResponse>> GetPaginatedForEmployeeAsync(long mentionedEmployeeId, PagedRequest request);
    }
}
