using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Application.Interfaces.Repositories
{
    public interface ISurveyRepository : IRepository<Survey>
    {
        Task<bool> ExistsByTitleAsync(string title, long? excludeId = null);
        Task<PagedResult<Survey>> GetPaginatedAsync(PagedRequest request);
    }
}
