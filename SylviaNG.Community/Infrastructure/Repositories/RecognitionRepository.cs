using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class RecognitionRepository : Repository<Recognition>, IRecognitionRepository
    {
        public RecognitionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Recognition>> GetPaginatedAsync(PagedRequest request, long? senderId = null, long? recipientId = null, long? viewerEmployeeId = null, bool viewerIsHrAdmin = false)
        {
            var query = _dbSet.AsQueryable();

            if (senderId.HasValue)
                query = query.Where(r => r.SenderId == senderId.Value);

            if (recipientId.HasValue)
                query = query.Where(r => r.RecipientId == recipientId.Value);

            // Private recognitions are only visible to their sender/recipient and HR/Admin -
            // everyone else only sees ones marked IsPublic.
            if (!viewerIsHrAdmin)
                query = query.Where(r => r.IsPublic || r.SenderId == viewerEmployeeId || r.RecipientId == viewerEmployeeId);

            request.SearchProperties ??= new[] { nameof(Recognition.AwardTitle), nameof(Recognition.Message) };

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
