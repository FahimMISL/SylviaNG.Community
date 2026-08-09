using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using SylviaNG.Community.SharedKernel.Pagination;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class ConversationRepository : Repository<Conversation>, IConversationRepository
    {
        public ConversationRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<PagedResult<Conversation>> GetPaginatedForEmployeeAsync(long employeeId, PagedRequest request)
        {
            var conversationIds = _dbContext.Set<ConversationParticipant>()
                .Where(cp => cp.EmployeeId == employeeId)
                .Select(cp => cp.ConversationId);

            var query = _dbSet.Where(c => conversationIds.Contains(c.ConversationId));

            return await query.ToPaginatedResultAsync(request);
        }
    }
}
