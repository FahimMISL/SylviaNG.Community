using Microsoft.EntityFrameworkCore;
using SylviaNG.Community.Application.Interfaces.Repositories;
using SylviaNG.Community.Domain.Entities;
using SylviaNG.Community.Infrastructure.Data;
using SylviaNG.Community.SharedKernel.Generic;
using Task = System.Threading.Tasks.Task;

namespace SylviaNG.Community.Infrastructure.Repositories
{
    public class CommentReactionRepository : Repository<CommentReaction>, ICommentReactionRepository
    {
        public CommentReactionRepository(ApplicationDBContext dbContext) : base(dbContext) { }

        public async Task<CommentReaction?> GetAsync(long commentId, long employeeId)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.CommentId == commentId && r.EmployeeId == employeeId);
        }

        public async Task<List<CommentReaction>> GetByCommentIdAsync(long commentId)
        {
            return await _dbSet.Where(r => r.CommentId == commentId).ToListAsync();
        }
    }
}
